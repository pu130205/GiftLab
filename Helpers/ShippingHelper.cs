using System.Globalization;
using System.Text;

namespace GiftLab.Helpers
{
    public static class ShippingHelper
    {
        public static int CalcShippingFee(string? cityInput)
            => IsHcm(cityInput) ? 22000 : 30000;

        // ✅ TPHCM / HCM / Hồ Chí Minh / Sài Gòn / Quận ... / Thủ Đức ... => 22k
        private static bool IsHcm(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            var s = RemoveDiacritics(input).ToLowerInvariant();

            s = s.Replace(".", " ").Replace(",", " ").Replace("-", " ").Replace("_", " ").Replace("/", " ");
            s = string.Join(" ", s.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            // TP.HCM variants
            if (s.Contains("tp hcm")) return true;

            if (s.Contains("tp.hcm")) return true;
            if (s.Contains("TP.HCM")) return true;
            if (s.Contains("Thành phố Hồ Chí Minh")) return true;
            if (s.Contains("Thanh pho Ho Chi Minh")) return true;
            if (s.Contains("ho chi minh")) return true;
            if (s.Contains("tphcm")) return true;
            if (s.Contains("hcm")) return true;
            if (s.Contains("Sai gon")) return true;
            if (s.Contains("Sài Gòn")) return true;
            if (s.Contains("saigon")) return true;

            // ✅ Nếu user/customer nhập kiểu "Quận 1", "Q1", "Thủ Đức", "TP Thủ Đức"...
            if (s.StartsWith("quan ")) return true;
            if (s.StartsWith("q ")) return true;
            if (s.StartsWith("q")) // q1, q7
            {
                // q1 q2 ... q12
                if (s.Length >= 2 && char.IsDigit(s[1])) return true;
            }
            if (s.Contains("thu duc")) return true;
            if (s.Contains("tp thu duc")) return true;

            return false;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            return sb.ToString()
                     .Replace('Đ', 'D')
                     .Replace('đ', 'd')
                     .Normalize(NormalizationForm.FormC);
        }
    }
}
