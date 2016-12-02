using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SasaraChanTalkTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var src = "http://example.com/";
			var dst = "URL省略。";

			Assert.AreEqual(SasaraChanTalk.mainViewModel.TrimText(src), dst);
		}
	}
}
