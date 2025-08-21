using Dapper;
using Mobile.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                string query = "SELECT id, employee_id AS EmployeeId, tanggal_lembur AS TanggalLembur, durasi::numeric AS Durasi, alasan, status, approved_by AS ApprovedBy, approved_at AS ApprovedAt, rejected_by AS RejectedBy, rejected_at AS RejectedAt, reject_reason AS RejectReason, date_created AS DateCreated FROM lembur";
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
                                rejected_by AS RejectedBy,
                                rejected_at AS RejectedAt,
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
                                    rejected_by = @RejectedBy,
                                    rejected_at = @RejectedAt,
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
        
        //untuk dapetin lembur yang difilter berdasarkan request
        //menggunakan Dapper untuk query ke database
        public async Task<IEnumerable<LemburFilterResponse>> GetFilteredLembur(LemburFilterRequest request)
        {
            using (IDbConnection conn = Connection)
            {
                var parameters = new Dapper.DynamicParameters();
                var whereClause = "";
                
                //menambahkan filter tanggal
                if (request.Start.HasValue)
                {
                    whereClause += " AND l.date_created >= @StartDate";
                    parameters.Add("StartDate", request.Start.Value);
                }
                
                if (request.End.HasValue)
                {
                    whereClause += " AND l.date_created <= @EndDate";
                    parameters.Add("EndDate", request.End.Value);
                }
                
                //menambahkan filter employeeId
                if (request.EmployeeId > 0)
                {
                    whereClause += " AND l.employee_id = @EmployeeId";
                    parameters.Add("EmployeeId", request.EmployeeId);
                }
                
                //menambahkan filter status
                if (request.Status > 0)
                {
                    // Convert new status codes to database values
                    int dbStatus = request.Status - 1; // 1->0 (Pending), 2->1 (Approved), 3->2 (Rejected)
                    whereClause += " AND l.status = @Status";
                    parameters.Add("Status", dbStatus);
                }
                else if (request.Status == 0)
                {
                    //untuk mengambil semua status
                }
                
                //menambahkan filter keyword
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    whereClause += " AND (l.alasan ILIKE @Keyword)";
                    parameters.Add("Keyword", $"%{request.Keyword}%");
                }
                
                //untuk mengkalkulasikan pagination
                var offset = (request.PageNumber - 1) * request.PageSize;
                
                //untuk mendapatkan total count
                var countParameters = new Dapper.DynamicParameters(parameters);
                string countQuery = $@"
                    SELECT COUNT(*)
                    FROM lembur l
                    LEFT JOIN employee e ON l.employee_id = e.id
                    WHERE 1=1 {whereClause}";
                
                var totalCount = await conn.QueryFirstOrDefaultAsync<int>(countQuery, countParameters);
                
                string query = $@"
                    SELECT
                        l.id,
                        COALESCE(e.companyid, 0) as CompanyId,
                        COALESCE(e.officeid, 0) as OfficeId,
                        l.employee_id as EmployeeId,
                        e.nik as NIK,
                        e.fullname as FullName,
                        e.position as Position,
                        e.phonenumber as PhoneNumber,
                        d.name as Division,
                        l.tanggal_lembur as TanggalLembur,
                        l.durasi as Durasi,
                        l.alasan as Alasan,
                        CASE
                            WHEN l.status = 0 THEN 'Pending'
                            WHEN l.status = 1 THEN 'Approved'
                            WHEN l.status = 2 THEN 'Rejected'
                            ELSE 'Unknown'
                        END as Status,
                        l.date_created as DateCreated,
                        @TotalCount as TotalCount,
                        a.fullname as UserApproved,
                        l.approved_at as DateApproved,
                        r.fullname as UserReject,
                        l.rejected_at as DateReject,
                        l.reject_reason as RejectNote
                    FROM lembur l
                    LEFT JOIN employee e ON l.employee_id = e.id
                    LEFT JOIN division d ON CAST(e.divisionid AS INTEGER) = CAST(d.id AS INTEGER)
                    LEFT JOIN employee a ON CAST(l.approved_by AS INTEGER) = CAST(a.id AS INTEGER)
                    LEFT JOIN employee r ON CAST(l.rejected_by AS INTEGER) = CAST(r.id AS INTEGER)
                    WHERE 1=1 {whereClause}
                    ORDER BY l.date_created DESC
                    LIMIT @Limit OFFSET @Offset";
                
                parameters.Add("TotalCount", totalCount);
                parameters.Add("Limit", request.PageSize);
                parameters.Add("Offset", offset);
                
                Console.WriteLine("Query: " + query);
                Console.WriteLine("Parameters: " + string.Join(", ", parameters.ParameterNames.Select(p => $"{p}={parameters.Get<object>(p)}")));
                
                return await conn.QueryAsync<LemburFilterResponse>(query, parameters);
            }
        }
    }
}