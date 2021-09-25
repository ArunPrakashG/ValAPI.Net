namespace ValAPINet {
	public class LoginInfo {
		public readonly string Username;
		public readonly string Password;

		LoginInfo(string userName, string password) {
			Username = userName;
			Password = password;
		}
	}
}
