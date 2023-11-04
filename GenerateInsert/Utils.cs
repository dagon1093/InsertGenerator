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

        /// <summary>
        /// Записывает строку <param name="stringToWrite">"stringToWrite"</param> в файл <param name="fileName">"fileName"</param>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stringToWrite"></param>
        /// <param name="append"></param>
        public static async void writeToFile(string fileName, string stringToWrite, bool append)
        {

            using (StreamWriter writer = new StreamWriter(fileName, append))
            {
                    await writer.WriteLineAsync(stringToWrite);

            }
        }

       
        

    }
}
