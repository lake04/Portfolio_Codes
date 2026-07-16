namespace Island
{
	public static class Utils
	{
		public static int PositiveMod(int value, int mod)
		{
			int r = value % mod;         // 나머지 계산 (음수일 수 있음)
			return r < 0 ? r + mod : r;  // 음수면 mod 더해서 양수로 변환
		}
	}
}