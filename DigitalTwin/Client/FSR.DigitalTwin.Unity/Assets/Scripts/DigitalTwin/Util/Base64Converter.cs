using System;
using System.Text;

namespace FSR.DigitalTwin.Util {

    public class Base64Converter {

        public static string ToBase64(string plainText) {
            byte[] bytesToEncode = Encoding.ASCII.GetBytes(plainText);
            return Convert.ToBase64String(bytesToEncode).Replace("=", "");
        }

        public static string FromBase64(string base64) {
            byte[] decodedBytes = Convert.FromBase64String(base64);
            return Encoding.ASCII.GetString(decodedBytes);
        }

    }

}