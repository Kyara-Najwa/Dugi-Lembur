using Dapper;
using Mobile.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Mobile.Services
{
    public class LemburService : GeneralService
    {
        //untuk dapetin semua lembur
        public async Task<IEnumerable<Lembur>> GetAll()
        {
            using (IDbConnection conn = Connection)
            {
                string query = "SELECT id, employee_id AS EmployeeId, tanggal_lembur AS TanggalLembur, durasi::numeric AS Durasi, alasan, status, approved_by AS ApprovedBy, approved_at AS ApprovedAt, reject_reason AS RejectReason, date_created AS DateCreated FROM lembur";
                return await conn.QueryAsync<Lembur>(query);
            }
        }

        //untuk dapetin lembur berdasarkan id
        //menggunakan Dapper untuk query ke database
        public async Task<Lembur> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"SELECT id, employee_id AS EmployeeId,
                                tanggal_lembur AS TanggalLembur,
                                durasi::numeric AS Durasi, alasan,
                                status, approved_by AS ApprovedBy,
                                approved_at AS ApprovedAt,
                                reject_reason AS RejectReason,
                                date_created AS DateCreated
                                FROM lembur WHERE id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Lembur>(query, new { Id = id });
            }
        }

        //untuk buat lembur baru
        public async Task<int> Create(Lembur lembur)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"INSERT INTO lembur (employee_id, tanggal_lembur, durasi, alasan, status, date_created)
                                VALUES (@EmployeeId, @TanggalLembur, @Durasi, @Alasan, @Status, @DateCreated)
                                RETURNING id";
                return await conn.ExecuteScalarAsync<int>(query, lembur);
            }
        }

        //untuk update lembur
        public async Task<bool> Update(Lembur lembur)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"UPDATE lembur
                                SET employee_id = @EmployeeId,
                                    tanggal_lembur = @TanggalLembur,
                                    durasi = @Durasi,
                                    alasan = @Alasan,
                                    status = @Status,
                                    approved_by = @ApprovedBy,
                                    approved_at = @ApprovedAt,
                                    reject_reason = @RejectReason,
                                    date_created = @DateCreated
                                WHERE id = @Id";
                var result = await conn.ExecuteAsync(query, lembur);
                return result > 0;
            }
        }
    
        //untuk dapetin employeeid dari email
        public async Task<int> GetEmployeeIdByEmail(string email)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"SELECT id FROM employee WHERE email = @Email AND IsDeleted = false";
                return await conn.QueryFirstOrDefaultAsync<int>(query, new { Email = email });
            }
        }
        
        //untuk dapetin employeeid dan roleid dari email
        public async Task<(int employeeId, int roleId)> GetEmployeeInfoByEmail(string email)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"SELECT id, employeeroleid FROM employee WHERE email = @Email AND IsDeleted = false";
                var result = await conn.QueryFirstOrDefaultAsync(query, new { Email = email });
                if (result != null)
                {
                    return (result.id, result.employeeroleid);
                }
                return (0, 0);
            }
        }
    }
}