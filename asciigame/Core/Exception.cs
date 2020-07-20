using System;

namespace asciigame.Core
{

	public class GameErrorException: Exception
	{
		public GameErrorException()
		{
		}

		public GameErrorException(string message)
			: base(message)
		{
		}

		public GameErrorException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	public class JsonErrorException: Exception
	{
		public JsonErrorException()
		{
		}

		public JsonErrorException(string message)
			: base(message)
		{
		}

		public JsonErrorException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}