using System.Data;
using bthrm.core.Data.Model;
using Dapper;

namespace bthrm.server.Database;

public class HeartRateRepository
{
    private readonly IDbConnection _db;

    public HeartRateRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<User?> GetUser(int userId)
    {
        var user = await _db.QueryFirstAsync<User>($"SELECT * FROM {TableNames.User} WHERE Id = @userId", new { userId });
        return user;
    }

    public async Task<HeartRateSession> CreateSession(HeartRateSession session)
    {
        var id = await _db.ExecuteScalarAsync<int>($@"
            INSERT INTO {TableNames.HeartRateSession}(UserId, StartDate) VALUES(@UserId, @StartDate);
            SELECT LAST_INSERT_ID();
        ", session);
        session.Id = id;
        session.Readings = new();

        return session;
    }

    public async Task<HeartRateSession?> GetSessionDetails(int sessionId)
    {
        var sessionReadings = new Dictionary<int, List<HeartRateReading>>();
        var sessions = await _db.QueryAsync($@"
            SELECT hrs.*, hrr.*
            FROM {TableNames.HeartRateSession} hrs
            LEFT JOIN {TableNames.HeartRateReading} hrr ON hrr.HeartRateSessionId = hrs.Id
            WHERE hrs.Id = @sessionId;"
        , (HeartRateSession hrs, HeartRateReading hrr) =>
        {
            if (sessionReadings.TryGetValue(hrs.Id, out var readings))
            {
                hrs.Readings = readings;
                hrs.Readings.Add(hrr);
            }
            else
            {
                hrs.Readings = sessionReadings[hrs.Id] = new List<HeartRateReading>
                {
                    hrr
                };
            }
            return hrs;
        }, new { sessionId });

        var session = sessions.FirstOrDefault();
        if (session != null)
        {
            session.Readings ??= new List<HeartRateReading>();
        }

        return session;
    }

    public async Task<HeartRateSession?> StopSession(HeartRateSession session)
    {
        await _db.ExecuteAsync($@"
            UPDATE {TableNames.HeartRateSession} 
            SET EndDate = @EndDate
            WHERE Id = @Id", session);

        return await _db.QueryFirstOrDefaultAsync<HeartRateSession>($@"SELECT * FROM {TableNames.HeartRateSession} WHERE Id = @Id", session);
    }

    public async Task<HeartRateReading> CreateReading(HeartRateReading reading)
    {
        var id = await _db.ExecuteScalarAsync<int>($@"
            INSERT INTO {TableNames.HeartRateReading}(HeartRateSessionId, Date, HeartRate) 
                VALUES(@HeartRateSessionId, @Date, @HeartRate);
            SELECT LAST_INSERT_ID();
        ", reading);
        reading.Id = id;

        return reading;
    }

    public async Task<HeartRateSession?> GetCurrentSession(int? userId)
    {
        if (userId == null) return null;

        var session = await _db.QueryFirstOrDefaultAsync<HeartRateSession>($@"
            SELECT * FROM {TableNames.HeartRateSession} s
            WHERE s.UserId = @userId
            AND NOW() >= s.StartDate
            AND (s.EndDate IS NULL);
        ", new { userId });

        return session;
    }
}