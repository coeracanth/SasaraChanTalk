﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CevioOutSideTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var src = "http://example.com/";
			var dst = "URL省略。";

			Assert.AreEqual(CevioOutSide.mainViewModel.TrimText(src), dst);
		}
	}
}
