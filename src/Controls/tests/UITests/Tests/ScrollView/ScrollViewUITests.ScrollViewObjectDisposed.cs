﻿using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests
{
	public class ScrollViewObjectDisposedUITests : ScrollViewUITests
	{
		public ScrollViewObjectDisposedUITests(TestDevice device)
			: base(device)
		{
		}

		// ScrollViewObjectDisposedTest (src\Compatibility\ControlGallery\src\Issues.Shared\ScrollViewObjectDisposed.cs)
		[Test]
		[Description("Tapping a button inside the ScrollView does not cause an exception.")]
		public void ScrollViewObjectDisposedTest()
		{
			this.IgnoreIfPlatforms(new TestDevice[] { TestDevice.Android },		
				"This test is failing, likely due to product issue");

			App.Click("ScrollViewNoObjectDisposed");

			App.Click("TestButtonId");
			App.WaitForElement("Success");
		}
	}
}