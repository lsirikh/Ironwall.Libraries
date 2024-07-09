namespace Ironwall.Framework.Models.Accounts
{
    public class UserModel 
        : UserBaseModel
        , IUserModel
    {
        public UserModel()
        {

        }

        public UserModel(IUserModel model)
            : base(model)
        {
            EmployeeNumber = model.EmployeeNumber;
            Birth = model.Birth;
            Phone = model.Phone;
            Address = model.Address;
            EMail = model.EMail;
            Image = model.Image;
            Position = model.Position;
            Department = model.Department;
            Company = model.Company;
        }

        public UserModel(int id, 
            string userId,
            string password,
            string name,
            string empnum,
            string birth,
            string phone,
            string address,
            string email,
            string image,
            string position,
            string department,
            string company,
            int level,
            bool used)
            :base(id, userId, password, level, name, used)
        {
            EmployeeNumber = empnum;
            Birth = birth;
            Phone = phone;
            Address = address;
            EMail = email;
            Image = image;
            Position = position;
            Department = department;
            Company = company;
        }

        #region - Implementations for IAccountModel -
        public string EmployeeNumber { get; set; }  //6
        public string Birth { get; set; }           //7
        public string Phone { get; set; }           //8
        public string Address { get; set; }         //9
        public string EMail { get; set; }           //10
        public string Image { get; set; }           //11
        public string Position { get; set; }        //12
        public string Department { get; set; }      //13
        public string Company { get; set; }         //14
        #endregion

        public void Insert(
            string userId, 
            string password, 
            string name, 
            string empnum, 
            string birth, 
            string phone, 
            string address, 
            string email, 
            string image, 
            string position, 
            string department, 
            string company, 
            int level, 
            bool used)
        {
            IdUser = userId;
            Password = password;
            Name = name;
            EmployeeNumber = empnum;
            Birth = birth;
            Phone = phone;
            Address = address;
            EMail = email;
            Image = image;
            Position = position;
            Department = department;
            Company = company;
            Level = level;
            Used = used;
        }

		public void Insert(IUserModel model)
		{
			Id = model.Id;
			IdUser = model.IdUser;
			Password = model.Password;
			Name = model.Name;
			EmployeeNumber = model.EmployeeNumber;
			Birth = model.Birth;
			Phone = model.Phone;
			Address = model.Address;
			EMail = model.EMail;
			Image = model.Image;
			Position = model.Position;
			Department = model.Department;
			Company = model.Company;
			Level = model.Level;
			Used = model.Used;
		}
	}
}
