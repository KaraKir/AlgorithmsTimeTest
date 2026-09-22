using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace AlgorithmsTimeTest
{
    public static class Database
    {
        private const string ConnStr = "Data Source=experiment.db;Version=3;";

        public static void Init()
        {
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS measurements (
                            id         INTEGER PRIMARY KEY AUTOINCREMENT,
                            algorithm  TEXT    NOT NULL,
                            n          INTEGER NOT NULL,
                            value      REAL    NOT NULL,
                            run_index  INTEGER NOT NULL,
                            timestamp  TEXT    NOT NULL
                        );
                        CREATE INDEX IF NOT EXISTS idx_meas
                            ON measurements(algorithm, n);

                        CREATE TABLE IF NOT EXISTS approximations (
                            id         INTEGER PRIMARY KEY AUTOINCREMENT,
                            algorithm  TEXT    NOT NULL,
                            model      TEXT    NOT NULL,
                            c          REAL    NOT NULL,
                            mse        REAL    NOT NULL,
                            timestamp  TEXT    NOT NULL
                        );";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============================================================
        // ====================   ЗАПИСЬ   ============================
        // ============================================================

        public static void SaveMeasurement(Measurement m)
        {
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO measurements
                        (algorithm, n, value, run_index, timestamp)
                        VALUES (@a, @n, @v, @r, @t)";
                    cmd.Parameters.AddWithValue("@a", m.Algorithm);
                    cmd.Parameters.AddWithValue("@n", m.N);
                    cmd.Parameters.AddWithValue("@v", m.Value);
                    cmd.Parameters.AddWithValue("@r", m.RunIndex);
                    cmd.Parameters.AddWithValue("@t", m.Timestamp.ToString("o"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void SaveApproximation(Approximation a)
        {
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO approximations
                        (algorithm, model, c, mse, timestamp)
                        VALUES (@a, @m, @c, @ms, @t)";
                    cmd.Parameters.AddWithValue("@a", a.Algorithm);
                    cmd.Parameters.AddWithValue("@m", a.Model);
                    cmd.Parameters.AddWithValue("@c", a.C);
                    cmd.Parameters.AddWithValue("@ms", a.MSE);
                    cmd.Parameters.AddWithValue("@t", a.Timestamp.ToString("o"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============================================================
        // ====================   ЧТЕНИЕ   ============================
        // ============================================================

        // Средние времена по n для одного алгоритма
        public static List<(int N, double Avg)> GetAverages(string algorithm)
        {
            var result = new List<(int, double)>();
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT n, AVG(value) FROM measurements
                                        WHERE algorithm = @a
                                        GROUP BY n ORDER BY n";
                    cmd.Parameters.AddWithValue("@a", algorithm);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            result.Add((r.GetInt32(0), r.GetDouble(1)));
                }
            }
            return result;
        }

        public static bool HasMeasurements(string algorithm)
        {
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT COUNT(*) FROM measurements WHERE algorithm = @a";
                    cmd.Parameters.AddWithValue("@a", algorithm);
                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        // Список всех алгоритмов, для которых есть замеры
        public static List<string> GetAllAlgorithmKeys()
        {
            var list = new List<string>();
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT DISTINCT algorithm FROM measurements
                                        ORDER BY algorithm";
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            list.Add(r.GetString(0));
                }
            }
            return list;
        }

        // Все записи аппроксимаций (для отчёта)
        public static List<Approximation> GetAllApproximations()
        {
            var list = new List<Approximation>();
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, algorithm, model, c, mse, timestamp
                                        FROM approximations
                                        ORDER BY timestamp DESC";
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                        {
                            list.Add(new Approximation
                            {
                                Id = r.GetInt64(0),
                                Algorithm = r.GetString(1),
                                Model = r.GetString(2),
                                C = r.GetDouble(3),
                                MSE = r.GetDouble(4),
                                Timestamp = DateTime.Parse(r.GetString(5))
                            });
                        }
                }
            }
            return list;
        }

        // ============================================================
        // ====================   УДАЛЕНИЕ   ==========================
        // ============================================================

        public static void ClearAlgorithm(string algorithm)
        {
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "DELETE FROM measurements WHERE algorithm = @a";
                    cmd.Parameters.AddWithValue("@a", algorithm);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Полная очистка обеих таблиц (для случая, если нужно начать заново)
        public static void ClearAll()
        {
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM measurements;
                        DELETE FROM approximations;";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}