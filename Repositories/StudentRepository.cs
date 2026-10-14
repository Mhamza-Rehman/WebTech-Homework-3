using System.Data;
using AttendanceSystem.Models;
using Microsoft.Data.SqlClient;

namespace AttendanceSystem.Repositories
{
    public class StudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AttendanceDB")
                ?? throw new InvalidOperationException("Connection string 'AttendanceDB' not found.");
        }

        // ---------- READ ALL ----------
        public async Task<List<Student>> GetAllAsync()
        {
            var table = new DataTable();

            using var conn = new SqlConnection(_connectionString);
            using var adapter = new SqlDataAdapter(
                "SELECT StudentId, StudentName, RegistrationNumber, Department, AttendancePercentage " +
                "FROM Students ORDER BY StudentId", conn);

            // SqlDataAdapter.Fill is synchronous, so run it off the request thread
            await Task.Run(() => adapter.Fill(table));

            return table.Rows.Cast<DataRow>().Select(MapRow).ToList();
        }

        // ---------- READ ONE ----------
        public async Task<Student?> GetByIdAsync(int id)
        {
            var table = new DataTable();

            using var conn = new SqlConnection(_connectionString);
            using var adapter = new SqlDataAdapter(
                "SELECT StudentId, StudentName, RegistrationNumber, Department, AttendancePercentage " +
                "FROM Students WHERE StudentId = @StudentId", conn);
            adapter.SelectCommand.Parameters.Add("@StudentId", SqlDbType.Int).Value = id;

            await Task.Run(() => adapter.Fill(table));

            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        // ---------- CREATE ----------
        public async Task<bool> AddAsync(Student student)
        {
            var table = new DataTable();

            using var conn = new SqlConnection(_connectionString);
            // WHERE 1 = 0 -> loads only the table structure, no rows
            using var adapter = new SqlDataAdapter(
                "SELECT StudentName, RegistrationNumber, Department, AttendancePercentage " +
                "FROM Students WHERE 1 = 0", conn);

            adapter.InsertCommand = new SqlCommand(
                "INSERT INTO Students (StudentName, RegistrationNumber, Department, AttendancePercentage) " +
                "VALUES (@StudentName, @RegistrationNumber, @Department, @AttendancePercentage)", conn);
            AddCommonParameters(adapter.InsertCommand);

            await Task.Run(() => adapter.Fill(table));

            var row = table.NewRow();
            row["StudentName"] = student.StudentName;
            row["RegistrationNumber"] = student.RegistrationNumber;
            row["Department"] = student.Department;
            row["AttendancePercentage"] = student.AttendancePercentage;
            table.Rows.Add(row);

            int affected = await Task.Run(() => adapter.Update(table));
            return affected > 0;
        }

        //  UPDATE
        public async Task<bool> UpdateAsync(Student student)
        {
            var table = new DataTable();

            using var conn = new SqlConnection(_connectionString);
            using var adapter = new SqlDataAdapter(
                "SELECT StudentId, StudentName, RegistrationNumber, Department, AttendancePercentage " +
                "FROM Students WHERE StudentId = @StudentId", conn);
            adapter.SelectCommand.Parameters.Add("@StudentId", SqlDbType.Int).Value = student.StudentId;

            adapter.UpdateCommand = new SqlCommand(
                "UPDATE Students SET StudentName = @StudentName, RegistrationNumber = @RegistrationNumber, " +
                "Department = @Department, AttendancePercentage = @AttendancePercentage " +
                "WHERE StudentId = @StudentId", conn);
            AddCommonParameters(adapter.UpdateCommand);
            var idParam = adapter.UpdateCommand.Parameters.Add("@StudentId", SqlDbType.Int, 0, "StudentId");
            idParam.SourceVersion = DataRowVersion.Original;

            await Task.Run(() => adapter.Fill(table));
            if (table.Rows.Count == 0) return false;

            var row = table.Rows[0];
            row["StudentName"] = student.StudentName;
            row["RegistrationNumber"] = student.RegistrationNumber;
            row["Department"] = student.Department;
            row["AttendancePercentage"] = student.AttendancePercentage;

            int affected = await Task.Run(() => adapter.Update(table));
            return affected > 0;
        }

        // DELETE ---
        public async Task<bool> DeleteAsync(int id)
        {
            var table = new DataTable();

            using var conn = new SqlConnection(_connectionString);
            using var adapter = new SqlDataAdapter(
                "SELECT StudentId FROM Students WHERE StudentId = @StudentId", conn);
            adapter.SelectCommand.Parameters.Add("@StudentId", SqlDbType.Int).Value = id;

            adapter.DeleteCommand = new SqlCommand(
                "DELETE FROM Students WHERE StudentId = @StudentId", conn);
            var idParam = adapter.DeleteCommand.Parameters.Add("@StudentId", SqlDbType.Int, 0, "StudentId");
            idParam.SourceVersion = DataRowVersion.Original;

            await Task.Run(() => adapter.Fill(table));
            if (table.Rows.Count == 0) return false;

            table.Rows[0].Delete();

            int affected = await Task.Run(() => adapter.Update(table));
            return affected > 0;
        }

        // HELPERS -
        private static void AddCommonParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@StudentName", SqlDbType.NVarChar, 100, "StudentName");
            cmd.Parameters.Add("@RegistrationNumber", SqlDbType.NVarChar, 30, "RegistrationNumber");
            cmd.Parameters.Add("@Department", SqlDbType.NVarChar, 50, "Department");

            var pct = new SqlParameter("@AttendancePercentage", SqlDbType.Decimal)
            {
                Precision = 5,
                Scale = 2,
                SourceColumn = "AttendancePercentage"
            };
            cmd.Parameters.Add(pct);
        }

        private static Student MapRow(DataRow row) => new Student
        {
            StudentId = Convert.ToInt32(row["StudentId"]),
            StudentName = row["StudentName"].ToString() ?? string.Empty,
            RegistrationNumber = row["RegistrationNumber"].ToString() ?? string.Empty,
            Department = row["Department"].ToString() ?? string.Empty,
            AttendancePercentage = Convert.ToDecimal(row["AttendancePercentage"])
        };
    }
}