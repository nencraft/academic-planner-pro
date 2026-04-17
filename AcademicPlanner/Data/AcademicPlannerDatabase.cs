using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using AcademicPlanner.Models;

namespace AcademicPlanner.Data
{
    public class AcademicPlannerDatabase
    {
        private SQLiteAsyncConnection? _database;
        private bool _initialized;

        private async Task InitAsync()
        {
            if (_initialized && _database is not null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "academicplanner.db3");

            _database = new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Term>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<Assessment>();
            await _database.CreateTableAsync<AppUser>();

            _initialized = true;
        }

        // terms
        public async Task<List<Term>> GetTermsAsync()
        {
            await InitAsync();
            return await _database!
                .Table<Term>()
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<Term?> GetTermAsync(int id)
        {
            await InitAsync();
            return await _database!
                .Table<Term>()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> SaveTermAsync(Term term)
        {
            await InitAsync();

            if (term.Id != 0)
                return await _database!.UpdateAsync(term);

            return await _database!.InsertAsync(term);
        }

        public async Task<int> DeleteTermAsync(Term term)
        {
            await InitAsync();
            return await _database!.DeleteAsync(term);
        }

        public async Task DeleteTermCascadeAsync(int termId)
        {
            await InitAsync();

            var courses = await GetCoursesByTermAsync(termId);
            foreach (var course in courses)
            {
                var assessments = await GetAssessmentsByCourseAsync(course.Id);
                foreach (var assessment in assessments)
                {
                    await _database!.DeleteAsync(assessment);
                }

                await _database!.DeleteAsync(course);
            }

            var term = await GetTermAsync(termId);
            if (term is not null)
            {
                await _database!.DeleteAsync(term);
            }
        }

        public async Task<Term?> GetOverlappingTermAsync(DateTime startDate, DateTime endDate, int currentTermId = 0)
        {
            await InitAsync();

            var terms = await _database!
                .Table<Term>()
                .Where(t => t.Id != currentTermId)
                .ToListAsync();

            return terms.FirstOrDefault(t =>
                startDate.Date < t.EndDate.Date &&
                endDate.Date > t.StartDate.Date);
        }

        // courses
        public async Task<int> SaveCourseAsync(Course course)
        {
            await InitAsync();

            if (course.Id != 0)
                return await _database!.UpdateAsync(course);

            return await _database!.InsertAsync(course);
        }

        public async Task<List<Course>> GetCoursesByTermAsync(int termId)
        {
            await InitAsync();
            return await _database!
                .Table<Course>()
                .Where(c => c.TermId == termId)
                .OrderBy(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseAsync(int id)
        {
            await InitAsync();
            return await _database!
                .Table<Course>()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<int> DeleteCourseAsync(Course course)
        {
            await InitAsync();
            return await _database!.DeleteAsync(course);
        }

        // assessments
        public async Task<List<Assessment>> GetAssessmentsByCourseAsync(int courseId)
        {
            await InitAsync();
            return await _database!
                .Table<Assessment>()
                .Where(a => a.CourseId == courseId)
                .OrderBy(a => a.Type)
                .ToListAsync();
        }

        public async Task<Assessment?> GetAssessmentAsync(int id)
        {
            await InitAsync();
            return await _database!
                .Table<Assessment>()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            await InitAsync();

            if (assessment.Id != 0)
                return await _database!.UpdateAsync(assessment);

            return await _database!.InsertAsync(assessment);
        }

        public async Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            await InitAsync();
            return await _database!.DeleteAsync(assessment);
        }

        public async Task DeleteCourseCascadeAsync(int courseId)
        {
            await InitAsync();

            var assessments = await GetAssessmentsByCourseAsync(courseId);
            foreach (var assessment in assessments)
            {
                await _database!.DeleteAsync(assessment);
            }

            var course = await GetCourseAsync(courseId);
            if (course is not null)
            {
                await _database!.DeleteAsync(course);
            }
        }

        // for sample data
        public async Task<bool> HasAnyDataAsync()
        {
            await InitAsync();

            int termCount = await _database!.Table<Term>().CountAsync();
            int courseCount = await _database!.Table<Course>().CountAsync();
            int assessmentCount = await _database!.Table<Assessment>().CountAsync();

            return termCount > 0 || courseCount > 0 || assessmentCount > 0;
        }

        // login auth
        public async Task<bool> HasUserAsync()
        {
            await InitAsync();
            return await _database!.Table<AppUser>().CountAsync() > 0;
        }

        public async Task<AppUser?> GetUserAsync()
        {
            await InitAsync();
            return await _database!.Table<AppUser>().FirstOrDefaultAsync();
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            await InitAsync();
            return await _database!
                .Table<AppUser>()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<int> SaveUserAsync(AppUser user)
        {
            await InitAsync();

            if (user.Id != 0)
                return await _database!.UpdateAsync(user);

            return await _database!.InsertAsync(user);
        }
    }
}
