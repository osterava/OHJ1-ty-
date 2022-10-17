using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using NUnit.Framework;
using static KauppaPeli.KauppaPeli;
using KauppaPeli;

namespace TestKauppaPeli
{
	[TestFixture]
	[DefaultFloatingPointTolerance(0.000001)]
	public  class TestKauppaPeli
	{
		[Test]
		public  void TestYmpyranAla105()
		{
			Assert.AreEqual( 12,566370614359, YmpyranAla(2) , 0.000001, "in method YmpyranAla, line 106");
			Assert.AreEqual( 50,265482457437, YmpyranAla(4) , 0.000001, "in method YmpyranAla, line 107");
			Assert.AreEqual( 0, YmpyranAla(0) , 0.000001, "in method YmpyranAla, line 108");
		}
	}
}

