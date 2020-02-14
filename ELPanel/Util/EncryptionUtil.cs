using System;
using System.Security.Cryptography;
using System.Text;

namespace ELPanel.Util {
    public static class EncryptionUtils {        public static string ComputeSha256Hash(string rawData) {
			using SHA256 sha = SHA256.Create();
			byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++) {
				builder.Append(bytes[i].ToString("x2"));
			}
			return builder.ToString();
		}        public static string ComputeSha384Hash(string rawData) {
			using SHA384 sha = SHA384.Create();
			byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++) {
				builder.Append(bytes[i].ToString("x2"));
			}
			return builder.ToString();
		}        public static string ComputeSha512Hash(string rawData) {
			using SHA512 sha = SHA512.Create();
			byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++) {
				builder.Append(bytes[i].ToString("x2"));
			}
			return builder.ToString();
		}        public static string ComputePassword(string password, string salt) {            return ComputeSha512Hash(ComputeSha384Hash(ComputeSha256Hash(password + salt) + password + salt) + password + salt);        }    }}