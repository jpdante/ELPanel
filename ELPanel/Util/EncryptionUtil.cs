using System;
using System.Security.Cryptography;
using System.Text;

namespace ELPanel.Util {
    public static class EncryptionUtils {        public static string ComputeSha256Hash(string rawData) {
			using var sha = SHA256.Create();
			byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			var builder = new StringBuilder();
			foreach (byte t in bytes) {
                builder.Append(t.ToString("x2"));
            }
			return builder.ToString();
		}        public static string ComputeSha384Hash(string rawData) {
			using var sha = SHA384.Create();
			byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			var builder = new StringBuilder();
			foreach (byte t in bytes) {
                builder.Append(t.ToString("x2"));
            }
			return builder.ToString();
		}        public static string ComputeSha512Hash(string rawData) {
			using var sha = SHA512.Create();
			byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			var builder = new StringBuilder();
			foreach (byte t in bytes) {
                builder.Append(t.ToString("x2"));
            }
			return builder.ToString();
		}        public static string ComputePassword(string password, string salt) {            return ComputeSha512Hash(ComputeSha384Hash(ComputeSha256Hash(password + salt) + password + salt) + password + salt);        }    }}