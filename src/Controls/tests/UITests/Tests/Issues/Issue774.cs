﻿using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests.Issues
{
	public class Issue774 : _IssuesUITest
	{
		public Issue774(TestDevice device) : base(device)
		{
		}

		public override string Issue => "ActionSheet won't dismiss after rotation to landscape";
		
		protected override void FixtureTeardown()
		{
			App.SetOrientationPortrait();

			base.FixtureTeardown();
		}

		[Test]
		[Category(UITestCategories.ActionSheet)]
		public void Issue774TestsDismissActionSheetAfterRotation()
		{
			this.IgnoreIfPlatforms(new TestDevice[] { TestDevice.iOS, TestDevice.Mac, TestDevice.Windows });

			App.Click("ShowActionSheet");
			App.Screenshot("Show ActionSheet");

			App.SetOrientationLandscape();
			App.Screenshot("Rotate Device");

			// Wait for the action sheet element to show up
			App.WaitForNoElement("What's up");

			var dismiss = App.FindElement("Dismiss");

			var target = dismiss != null ? "Dismiss" : "Destroy";

			App.Back();
			App.WaitForNoElement(target);

			App.Screenshot("Dismiss ActionSheet");
		}
	}
}
