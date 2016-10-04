using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// http://www.cl.cam.ac.uk/~mgk25/ucs/wcwidth.c
/// </summary>
namespace Ntreev.Library.Commands
{
    struct Interval
    {
        public Interval(int first, int last)
        {
            this.first = first;
            this.last = last;
        }
        public int first;
        public int last;
    };

    static class CharWidth
    {
        #region variables

        private static readonly Interval[] combining = {
    new Interval( 0x0300, 0x036F ), new Interval( 0x0483, 0x0486 ), new Interval( 0x0488, 0x0489 ),
    new Interval( 0x0591, 0x05BD ), new Interval( 0x05BF, 0x05BF ), new Interval( 0x05C1, 0x05C2 ),
    new Interval( 0x05C4, 0x05C5 ), new Interval( 0x05C7, 0x05C7 ), new Interval( 0x0600, 0x0603 ),
    new Interval( 0x0610, 0x0615 ), new Interval( 0x064B, 0x065E ), new Interval( 0x0670, 0x0670 ),
    new Interval( 0x06D6, 0x06E4 ), new Interval( 0x06E7, 0x06E8 ), new Interval( 0x06EA, 0x06ED ),
    new Interval( 0x070F, 0x070F ), new Interval( 0x0711, 0x0711 ), new Interval( 0x0730, 0x074A ),
    new Interval( 0x07A6, 0x07B0 ), new Interval( 0x07EB, 0x07F3 ), new Interval( 0x0901, 0x0902 ),
    new Interval( 0x093C, 0x093C ), new Interval( 0x0941, 0x0948 ), new Interval( 0x094D, 0x094D ),
    new Interval( 0x0951, 0x0954 ), new Interval( 0x0962, 0x0963 ), new Interval( 0x0981, 0x0981 ),
    new Interval( 0x09BC, 0x09BC ), new Interval( 0x09C1, 0x09C4 ), new Interval( 0x09CD, 0x09CD ),
    new Interval( 0x09E2, 0x09E3 ), new Interval( 0x0A01, 0x0A02 ), new Interval( 0x0A3C, 0x0A3C ),
    new Interval( 0x0A41, 0x0A42 ), new Interval( 0x0A47, 0x0A48 ), new Interval( 0x0A4B, 0x0A4D ),
    new Interval( 0x0A70, 0x0A71 ), new Interval( 0x0A81, 0x0A82 ), new Interval( 0x0ABC, 0x0ABC ),
    new Interval( 0x0AC1, 0x0AC5 ), new Interval( 0x0AC7, 0x0AC8 ), new Interval( 0x0ACD, 0x0ACD ),
    new Interval( 0x0AE2, 0x0AE3 ), new Interval( 0x0B01, 0x0B01 ), new Interval( 0x0B3C, 0x0B3C ),
    new Interval( 0x0B3F, 0x0B3F ), new Interval( 0x0B41, 0x0B43 ), new Interval( 0x0B4D, 0x0B4D ),
    new Interval( 0x0B56, 0x0B56 ), new Interval( 0x0B82, 0x0B82 ), new Interval( 0x0BC0, 0x0BC0 ),
    new Interval( 0x0BCD, 0x0BCD ), new Interval( 0x0C3E, 0x0C40 ), new Interval( 0x0C46, 0x0C48 ),
    new Interval( 0x0C4A, 0x0C4D ), new Interval( 0x0C55, 0x0C56 ), new Interval( 0x0CBC, 0x0CBC ),
    new Interval( 0x0CBF, 0x0CBF ), new Interval( 0x0CC6, 0x0CC6 ), new Interval( 0x0CCC, 0x0CCD ),
    new Interval( 0x0CE2, 0x0CE3 ), new Interval( 0x0D41, 0x0D43 ), new Interval( 0x0D4D, 0x0D4D ),
    new Interval( 0x0DCA, 0x0DCA ), new Interval( 0x0DD2, 0x0DD4 ), new Interval( 0x0DD6, 0x0DD6 ),
    new Interval( 0x0E31, 0x0E31 ), new Interval( 0x0E34, 0x0E3A ), new Interval( 0x0E47, 0x0E4E ),
    new Interval( 0x0EB1, 0x0EB1 ), new Interval( 0x0EB4, 0x0EB9 ), new Interval( 0x0EBB, 0x0EBC ),
    new Interval( 0x0EC8, 0x0ECD ), new Interval( 0x0F18, 0x0F19 ), new Interval( 0x0F35, 0x0F35 ),
    new Interval( 0x0F37, 0x0F37 ), new Interval( 0x0F39, 0x0F39 ), new Interval( 0x0F71, 0x0F7E ),
    new Interval( 0x0F80, 0x0F84 ), new Interval( 0x0F86, 0x0F87 ), new Interval( 0x0F90, 0x0F97 ),
    new Interval( 0x0F99, 0x0FBC ), new Interval( 0x0FC6, 0x0FC6 ), new Interval( 0x102D, 0x1030 ),
    new Interval( 0x1032, 0x1032 ), new Interval( 0x1036, 0x1037 ), new Interval( 0x1039, 0x1039 ),
    new Interval( 0x1058, 0x1059 ), new Interval( 0x1160, 0x11FF ), new Interval( 0x135F, 0x135F ),
    new Interval( 0x1712, 0x1714 ), new Interval( 0x1732, 0x1734 ), new Interval( 0x1752, 0x1753 ),
    new Interval( 0x1772, 0x1773 ), new Interval( 0x17B4, 0x17B5 ), new Interval( 0x17B7, 0x17BD ),
    new Interval( 0x17C6, 0x17C6 ), new Interval( 0x17C9, 0x17D3 ), new Interval( 0x17DD, 0x17DD ),
    new Interval( 0x180B, 0x180D ), new Interval( 0x18A9, 0x18A9 ), new Interval( 0x1920, 0x1922 ),
    new Interval( 0x1927, 0x1928 ), new Interval( 0x1932, 0x1932 ), new Interval( 0x1939, 0x193B ),
    new Interval( 0x1A17, 0x1A18 ), new Interval( 0x1B00, 0x1B03 ), new Interval( 0x1B34, 0x1B34 ),
    new Interval( 0x1B36, 0x1B3A ), new Interval( 0x1B3C, 0x1B3C ), new Interval( 0x1B42, 0x1B42 ),
    new Interval( 0x1B6B, 0x1B73 ), new Interval( 0x1DC0, 0x1DCA ), new Interval( 0x1DFE, 0x1DFF ),
    new Interval( 0x200B, 0x200F ), new Interval( 0x202A, 0x202E ), new Interval( 0x2060, 0x2063 ),
    new Interval( 0x206A, 0x206F ), new Interval( 0x20D0, 0x20EF ), new Interval( 0x302A, 0x302F ),
    new Interval( 0x3099, 0x309A ), new Interval( 0xA806, 0xA806 ), new Interval( 0xA80B, 0xA80B ),
    new Interval( 0xA825, 0xA826 ), new Interval( 0xFB1E, 0xFB1E ), new Interval( 0xFE00, 0xFE0F ),
    new Interval( 0xFE20, 0xFE23 ), new Interval( 0xFEFF, 0xFEFF ), new Interval( 0xFFF9, 0xFFFB ),
    new Interval( 0x10A01, 0x10A03 ), new Interval( 0x10A05, 0x10A06 ), new Interval( 0x10A0C, 0x10A0F ),
    new Interval( 0x10A38, 0x10A3A ), new Interval( 0x10A3F, 0x10A3F ), new Interval( 0x1D167, 0x1D169 ),
    new Interval( 0x1D173, 0x1D182 ), new Interval( 0x1D185, 0x1D18B ), new Interval( 0x1D1AA, 0x1D1AD ),
    new Interval( 0x1D242, 0x1D244 ), new Interval( 0xE0001, 0xE0001 ), new Interval( 0xE0020, 0xE007F ),
    new Interval( 0xE0100, 0xE01EF )
  };

        static readonly Interval[] ambiguous = {
    new Interval( 0x00A1, 0x00A1 ), new Interval( 0x00A4, 0x00A4 ), new Interval( 0x00A7, 0x00A8 ),
    new Interval( 0x00AA, 0x00AA ), new Interval( 0x00AE, 0x00AE ), new Interval( 0x00B0, 0x00B4 ),
    new Interval( 0x00B6, 0x00BA ), new Interval( 0x00BC, 0x00BF ), new Interval( 0x00C6, 0x00C6 ),
    new Interval( 0x00D0, 0x00D0 ), new Interval( 0x00D7, 0x00D8 ), new Interval( 0x00DE, 0x00E1 ),
    new Interval( 0x00E6, 0x00E6 ), new Interval( 0x00E8, 0x00EA ), new Interval( 0x00EC, 0x00ED ),
    new Interval( 0x00F0, 0x00F0 ), new Interval( 0x00F2, 0x00F3 ), new Interval( 0x00F7, 0x00FA ),
    new Interval( 0x00FC, 0x00FC ), new Interval( 0x00FE, 0x00FE ), new Interval( 0x0101, 0x0101 ),
    new Interval( 0x0111, 0x0111 ), new Interval( 0x0113, 0x0113 ), new Interval( 0x011B, 0x011B ),
    new Interval( 0x0126, 0x0127 ), new Interval( 0x012B, 0x012B ), new Interval( 0x0131, 0x0133 ),
    new Interval( 0x0138, 0x0138 ), new Interval( 0x013F, 0x0142 ), new Interval( 0x0144, 0x0144 ),
    new Interval( 0x0148, 0x014B ), new Interval( 0x014D, 0x014D ), new Interval( 0x0152, 0x0153 ),
    new Interval( 0x0166, 0x0167 ), new Interval( 0x016B, 0x016B ), new Interval( 0x01CE, 0x01CE ),
    new Interval( 0x01D0, 0x01D0 ), new Interval( 0x01D2, 0x01D2 ), new Interval( 0x01D4, 0x01D4 ),
    new Interval( 0x01D6, 0x01D6 ), new Interval( 0x01D8, 0x01D8 ), new Interval( 0x01DA, 0x01DA ),
    new Interval( 0x01DC, 0x01DC ), new Interval( 0x0251, 0x0251 ), new Interval( 0x0261, 0x0261 ),
    new Interval( 0x02C4, 0x02C4 ), new Interval( 0x02C7, 0x02C7 ), new Interval( 0x02C9, 0x02CB ),
    new Interval( 0x02CD, 0x02CD ), new Interval( 0x02D0, 0x02D0 ), new Interval( 0x02D8, 0x02DB ),
    new Interval( 0x02DD, 0x02DD ), new Interval( 0x02DF, 0x02DF ), new Interval( 0x0391, 0x03A1 ),
    new Interval( 0x03A3, 0x03A9 ), new Interval( 0x03B1, 0x03C1 ), new Interval( 0x03C3, 0x03C9 ),
    new Interval( 0x0401, 0x0401 ), new Interval( 0x0410, 0x044F ), new Interval( 0x0451, 0x0451 ),
    new Interval( 0x2010, 0x2010 ), new Interval( 0x2013, 0x2016 ), new Interval( 0x2018, 0x2019 ),
    new Interval( 0x201C, 0x201D ), new Interval( 0x2020, 0x2022 ), new Interval( 0x2024, 0x2027 ),
    new Interval( 0x2030, 0x2030 ), new Interval( 0x2032, 0x2033 ), new Interval( 0x2035, 0x2035 ),
    new Interval( 0x203B, 0x203B ), new Interval( 0x203E, 0x203E ), new Interval( 0x2074, 0x2074 ),
    new Interval( 0x207F, 0x207F ), new Interval( 0x2081, 0x2084 ), new Interval( 0x20AC, 0x20AC ),
    new Interval( 0x2103, 0x2103 ), new Interval( 0x2105, 0x2105 ), new Interval( 0x2109, 0x2109 ),
    new Interval( 0x2113, 0x2113 ), new Interval( 0x2116, 0x2116 ), new Interval( 0x2121, 0x2122 ),
    new Interval( 0x2126, 0x2126 ), new Interval( 0x212B, 0x212B ), new Interval( 0x2153, 0x2154 ),
    new Interval( 0x215B, 0x215E ), new Interval( 0x2160, 0x216B ), new Interval( 0x2170, 0x2179 ),
    new Interval( 0x2190, 0x2199 ), new Interval( 0x21B8, 0x21B9 ), new Interval( 0x21D2, 0x21D2 ),
    new Interval( 0x21D4, 0x21D4 ), new Interval( 0x21E7, 0x21E7 ), new Interval( 0x2200, 0x2200 ),
    new Interval( 0x2202, 0x2203 ), new Interval( 0x2207, 0x2208 ), new Interval( 0x220B, 0x220B ),
    new Interval( 0x220F, 0x220F ), new Interval( 0x2211, 0x2211 ), new Interval( 0x2215, 0x2215 ),
    new Interval( 0x221A, 0x221A ), new Interval( 0x221D, 0x2220 ), new Interval( 0x2223, 0x2223 ),
    new Interval( 0x2225, 0x2225 ), new Interval( 0x2227, 0x222C ), new Interval( 0x222E, 0x222E ),
    new Interval( 0x2234, 0x2237 ), new Interval( 0x223C, 0x223D ), new Interval( 0x2248, 0x2248 ),
    new Interval( 0x224C, 0x224C ), new Interval( 0x2252, 0x2252 ), new Interval( 0x2260, 0x2261 ),
    new Interval( 0x2264, 0x2267 ), new Interval( 0x226A, 0x226B ), new Interval( 0x226E, 0x226F ),
    new Interval( 0x2282, 0x2283 ), new Interval( 0x2286, 0x2287 ), new Interval( 0x2295, 0x2295 ),
    new Interval( 0x2299, 0x2299 ), new Interval( 0x22A5, 0x22A5 ), new Interval( 0x22BF, 0x22BF ),
    new Interval( 0x2312, 0x2312 ), new Interval( 0x2460, 0x24E9 ), new Interval( 0x24EB, 0x254B ),
    new Interval( 0x2550, 0x2573 ), new Interval( 0x2580, 0x258F ), new Interval( 0x2592, 0x2595 ),
    new Interval( 0x25A0, 0x25A1 ), new Interval( 0x25A3, 0x25A9 ), new Interval( 0x25B2, 0x25B3 ),
    new Interval( 0x25B6, 0x25B7 ), new Interval( 0x25BC, 0x25BD ), new Interval( 0x25C0, 0x25C1 ),
    new Interval( 0x25C6, 0x25C8 ), new Interval( 0x25CB, 0x25CB ), new Interval( 0x25CE, 0x25D1 ),
    new Interval( 0x25E2, 0x25E5 ), new Interval( 0x25EF, 0x25EF ), new Interval( 0x2605, 0x2606 ),
    new Interval( 0x2609, 0x2609 ), new Interval( 0x260E, 0x260F ), new Interval( 0x2614, 0x2615 ),
    new Interval( 0x261C, 0x261C ), new Interval( 0x261E, 0x261E ), new Interval( 0x2640, 0x2640 ),
    new Interval( 0x2642, 0x2642 ), new Interval( 0x2660, 0x2661 ), new Interval( 0x2663, 0x2665 ),
    new Interval( 0x2667, 0x266A ), new Interval( 0x266C, 0x266D ), new Interval( 0x266F, 0x266F ),
    new Interval( 0x273D, 0x273D ), new Interval( 0x2776, 0x277F ), new Interval( 0xE000, 0xF8FF ),
    new Interval( 0xFFFD, 0xFFFD ), new Interval( 0xF0000, 0xFFFFD ), new Interval( 0x100000, 0x10FFFD )
  };

        #endregion

        private static bool bisearch(char ucs, Interval[] tables)
        {
            int min = 0;
            int max = tables.Length - 1;
            int mid;

            if (ucs < tables[0].first || ucs > tables[max].last)
                return false;
            while (max >= min)
            {
                mid = (min + max) / 2;
                if (ucs > tables[mid].last)
                    min = mid + 1;
                else if (ucs < tables[mid].first)
                    max = mid - 1;
                else
                    return true;
            }

            return false;
        }

        public static int mk_wcwidth(char ucs)
        {
            /* sorted list of non-overlapping intervals of non-spacing characters */
            /* generated by "uniset +cat=Me +cat=Mn +cat=Cf -00AD +1160-11FF +200B c" */


            /* test for 8-bit control characters */
            if (ucs == 0)
                return 0;
            if (ucs < 32 || (ucs >= 0x7f && ucs < 0xa0))
                return -1;

            /* binary search in table of non-spacing characters */
            if (bisearch(ucs, combining))
                return 0;

            /* if we arrive here, ucs is not a combining or C0/C1 control character */


            if ((Convert.ToInt32(ucs) >= 0x1100 &&
               (Convert.ToInt32(ucs) <= 0x115f ||                    /* Hangul Jamo init. consonants */
                Convert.ToInt32(ucs) == 0x2329 || Convert.ToInt32(ucs) == 0x232a ||
                (Convert.ToInt32(ucs) >= 0x2e80 && Convert.ToInt32(ucs) <= 0xa4cf &&
                 Convert.ToInt32(ucs) != 0x303f) ||                  /* CJK ... Yi */
                (Convert.ToInt32(ucs) >= 0xac00 && Convert.ToInt32(ucs) <= 0xd7a3) || /* Hangul Syllables */
                (Convert.ToInt32(ucs) >= 0xf900 && Convert.ToInt32(ucs) <= 0xfaff) || /* CJK Compatibility Ideographs */
                (Convert.ToInt32(ucs) >= 0xfe10 && Convert.ToInt32(ucs) <= 0xfe19) || /* Vertical forms */
                (Convert.ToInt32(ucs) >= 0xfe30 && Convert.ToInt32(ucs) <= 0xfe6f) || /* CJK Compatibility Forms */
                (Convert.ToInt32(ucs) >= 0xff00 && Convert.ToInt32(ucs) <= 0xff60) || /* Fullwidth Forms */
                (Convert.ToInt32(ucs) >= 0xffe0 && Convert.ToInt32(ucs) <= 0xffe6) ||
                (Convert.ToInt32(ucs) >= 0x20000 && Convert.ToInt32(ucs) <= 0x2fffd) ||
                (Convert.ToInt32(ucs) >= 0x30000 && Convert.ToInt32(ucs) <= 0x3fffd))))
                return 2;
            return 1;
        }

        public static int mk_wcswidth(string pwcs)
        {
            int w, width = 0;

            foreach (var item in pwcs)
                if ((w = mk_wcwidth(item)) < 0)
                    return -1;
                else
                    width += w;

            return width;
        }

        public static int mk_wcwidth_cjk(char ucs)
        {
            /* sorted list of non-overlapping intervals of East Asian Ambiguous
             * characters, generated by "uniset +WIDTH-A -cat=Me -cat=Mn -cat=Cf c" */


            /* binary search in table of non-spacing characters */
            if (bisearch(ucs, ambiguous))
                return 2;

            return mk_wcwidth(ucs);
        }

        public static int mk_wcswidth_cjk(string pwcs)
        {
            int w, width = 0;

            foreach (var item in pwcs)
                if ((w = mk_wcwidth_cjk(item)) < 0)
                    return -1;
                else
                    width += w;

            return width;
        }
    }

}
