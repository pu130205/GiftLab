using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data
{
    public static class SqlSeedRunner
    {
        private static readonly Regex GoRegex =
            new(@"^\s*GO\s*;$|^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        // ✅ Chỉ nhận 2 tham số (db + relativePath)
        public static void RunSqlFile(GiftLabDbContext db, string relativePath)
        {
            // 1) Thử trong folder chạy app (bin\Debug\net8.0\...)
            var path1 = Path.Combine(AppContext.BaseDirectory, relativePath);

            // 2) Thử trong folder project (thường là root khi chạy bằng VS)
            var path2 = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            string? fullPath = null;

            if (File.Exists(path1)) fullPath = path1;
            else if (File.Exists(path2)) fullPath = path2;

            if (fullPath == null)
                throw new FileNotFoundException(
                    $"Không tìm thấy file seed SQL. Đã thử:\n- {path1}\n- {path2}"
                );

            var sql = File.ReadAllText(fullPath);

            var batches = GoRegex.Split(sql)
                                 .Select(x => x.Trim())
                                 .Where(x => !string.IsNullOrWhiteSpace(x));

            foreach (var batch in batches)
            {
                db.Database.ExecuteSqlRaw(batch);
            }
        }
    }
}
