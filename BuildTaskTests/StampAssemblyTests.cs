﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Palaso.BuildTasks.StampAssemblies;

namespace BuildTaskTests
{
	[TestFixture]
	public class StampAssemblyTests
	{

		[Test]
		public void GetExistingAssemblyVersion_Normal_GetsAllFourParts()
		{
			var stamper = new StampAssemblies();
			var content = @"// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion(""1.*.3.44"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]";

			var v = stamper.GetExistingAssemblyVersion(content);
			Assert.AreEqual("1", v.parts[0]);
			Assert.AreEqual("*", v.parts[1]);
			Assert.AreEqual("3", v.parts[2]);
			Assert.AreEqual("44", v.parts[3]);
		}

		[Test]
		public void GetModifiedContents_LastTwoLeftToBuildScript_CorrectMerge()
		{
			var stamper = new StampAssemblies();
			var content = @"// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion(""0.7.*.*"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]";

			var s = stamper.GetModifiedContents(content, "*.*.123.456");
			Assert.IsTrue(s.Contains("0.7.123.456"));
		}

		[Test]
		public void GetModifiedContents_LastPartIsHash_HashReplacedWithZero()
		{
			var stamper = new StampAssemblies();
			var content = @"// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion(""0.7.*.0"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]";

			var s = stamper.GetModifiedContents(content, "*.*.123.9e1b12ec3712");
			Assert.IsTrue(s.Contains("0.7.123.0"));
		}

		/// <summary>
		/// Regression test... for some reason team city gives us assemblyInfo's like this
		/// </summary>
		[Test]
		public void GetModifiedContents_MissingAssemblyVersion_StillGetValidVersion()
		{
			var stamper = new StampAssemblies();
			var content =
				@"// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion(""..."")]
[assembly: AssemblyFileVersion(""..."")]";

			var s = stamper.GetModifiedContents(content, "*.*.123.0");
			Assert.IsTrue(s.Contains("0.0.123.0"));
		}
	}
}
