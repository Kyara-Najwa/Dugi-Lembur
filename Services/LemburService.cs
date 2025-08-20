using Dapper;
using Mobile.Model;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Mobile.Services
{
    public class LemburService : GeneralService
    {
        public async Task<IEnumerable<Lembur>> GetAll()
        {
            using (IDbConnection conn = Connection)
            {
                string query = "SELECT id, employee_id AS EmployeeId, tanggal_lembur AS TanggalLembur, durasi::numeric AS Durasi, alasan FROM lembur";
                return await conn.QueryAsync<Lembur>(query);
            }
        }

        public async Task<Lembur> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"SELECT id, employee_id AS EmployeeId,
                                tanggal_lembur AS TanggalLembur,
                                durasi::numeric AS Durasi, alasan FROM lembur WHERE id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Lembur>(query, new { Id = id });
            }
        }

        public async Task<int> Create(Lembur lembur)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"INSERT INTO lembur (employee_id, tanggal_lembur, durasi, alasan)
                                VALUES (@EmployeeId, @TanggalLembur, @Durasi, @Alasan)
                                RETURNING id";
                return await conn.ExecuteScalarAsync<int>(query, lembur);
            }
        }

        public async Task<bool> Update(Lembur lembur)
        {
            using (IDbConnection conn = Connection)
            {
                string query = @"UPDATE lembur
                                SET employee_id = @EmployeeId,
                                tanggal_lembur = @TanggalLembur,
                                durasi = @Durasi,
                                alasan = @Alasan
                                WHERE id = @Id";
                var result = await conn.ExecuteAsync(query, lembur);
                return result > 0;
            }
        }
    }
}