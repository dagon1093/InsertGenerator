using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace GenerateInsert
{
    static class Utils
    {
        /// <summary>
        /// Возвращает список имен столбцов таблицы <param name="tableName">"tableName"</param>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetTableColumnsAsync(NpgsqlConnection connection, string tableName)
        {
            var columns = new List<string>();
            string cmdColumnNames = $"SELECT column_name\r\nFROM information_schema.columns\r\nWHERE table_schema='public' AND table_name='{tableName}' ";

            await using (NpgsqlCommand command = new NpgsqlCommand(cmdColumnNames, connection))
            {
                await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        columns.Add(reader.GetString(0));
                    }
                }
                //todo log4net Console.WriteLine($"Columns ({string.Join(", ", columns)})");
                return columns;

            }
        }

    }
}
