﻿using MySql.Data.MySqlClient;using System;using System.Collections.Generic;using System.Data.Common;using System.Reflection;using System.Text;using System.Threading.Tasks;using HtcSharp.Core.Logging.Abstractions;namespace ELPanel.Manager {
    public class MySqlManager : IDisposable {        private readonly string _connectString;        public MySqlManager(string connString) {            _connectString = connString;        }        public async Task<MySqlConnection> GetMySqlConnection() {            try {                var conn = new MySqlConnection(_connectString);                await conn.OpenAsync();                return conn;            } catch (Exception ex) {                HtcPlugin.Logger.LogTrace("Could not connect to the database!", ex);                await Task.Delay(TimeSpan.FromSeconds(3));                return null;            }        }        public async Task<bool> CheckLogin(string email, string password, MySqlConnection conn) {            await using var cmd = new MySqlCommand("SELECT `id` FROM `accounts` WHERE `email` = @email AND `password` = @password;", conn);            cmd.Parameters.AddWithValue("email", email);            cmd.Parameters.AddWithValue("password", password);            await using var reader = await cmd.ExecuteReaderAsync();            return reader.HasRows;        }        public async Task CloseMySqlConnection(MySqlConnection connection) {            await connection.CloseAsync();            await connection.DisposeAsync();        }

        public async Task<ServerInfo[]> GetServers(MySqlConnection conn) {            var serverInfos = new List<ServerInfo>();            await using (var cmd = new MySqlCommand("SELECT * FROM `servers`;", conn)) {                await using var reader = await cmd.ExecuteReaderAsync();                if (!reader.HasRows) return serverInfos.ToArray();                while (reader.Read()) {                    serverInfos.Add(new ServerInfo(                        reader.GetInt32(0),                        reader.GetString(1),                        reader.GetString(2),                        reader.GetString(3),                        reader.GetString(4),                        reader.GetInt32(5),                        reader.GetInt32(6)                    ));                }            }            return serverInfos.ToArray();        }        public void Dispose() {            GC.SuppressFinalize(this);        }    }}