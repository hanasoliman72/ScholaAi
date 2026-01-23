using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.DTOs.User;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Services.User
{
    public class userRegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;

       public userRegisterService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
        }
        public async Task<studentRegisterDto> registerStudent(studentRegisterDto nUser)
        {
            //if (nUser == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //var existingUser = await _userRepository.getByEmailAsync(nUser.email);
            //if (existingUser != null)
            //{
            //    throw new Exception("Email already exists");
            //}

            var newUser = new user
            {
                userName = nUser.userName,
                email = nUser.email,
                firstName = nUser.firstName,
                lastName = nUser.lastName,
                phone = nUser.phone,
                description = nUser.description,
              
                profilePhotoURL = nUser.profilePhotoURL,
                applicationUserId = nUser.id,
                userType = ScholaAi.Models.Type.Student

                //passwordHash = null // Password managed by Identity, not stored in this table
            };

            await _userRepository.addAsync(newUser);

            var student = new student
            {
                userId =newUser.userId,
                grade = nUser.grade,
            };
            await _studentRepository.addAsync(student);

            nUser.userId = newUser.userId;

            return nUser;
        }

        public async Task<teacherRegisterDto> registerTeacher(teacherRegisterDto nUser)
        {
            //if (nUser == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //var existingUser = await _userRepository.getByEmailAsync(nUser.email);
            //if (existingUser != null)
            //{
            //    throw new Exception("Email already exists");
            //}

            var newUser = new user
            {
                userName = nUser.userName,
                email = nUser.email,
                firstName = nUser.firstName,
                lastName = nUser.lastName,
                phone = nUser.phone,
                description = nUser.description,
                // Don't store passwordHash here - Identity handles it in applicationUser
                // The applicationUserId links to Identity user
                profilePhotoURL = nUser.profilePhotoURL,
                applicationUserId = nUser.id,
                userType = ScholaAi.Models.Type.Teacher
                //passwordHash = null // Password managed by Identity, not stored in this table
            };
            await _userRepository.addAsync(newUser);
            var teacher = new teacher
            {
                userId = newUser.userId,
                certificate =nUser.certificate,
                college = nUser.college

            };
            await _teacherRepository.addAsync(teacher);

            nUser.userId = newUser.userId;

            return nUser;
        }

        public async Task<user> GetUserByApplicationUserId(string appUserId)
        {
            return await _userRepository.GetUserByApplicationUserId(appUserId);
        }

    }
}
