using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace KauppaPeli;

public class KauppaPeli : PhysicsGame
{
    public override void Begin()
    {
        Level.Background.Color = Color.Black;
        Level.CreateBorders();
        for (int i = 0; i < 10; i++)
        {
            LuoSatunainenYmpyra(this, Level.Left, Level.Bottom, Level.Right, 0);

        }
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

    }
   public static void LuoSatunainenYmpyra(PhysicsGame peli,double pieninx,double pieniny,double isoinx,double isoinY)
    {
        double leveys = RandomGen.NextDouble(40, 40);
        double korkeus = RandomGen.NextDouble(40, 40);  
        PhysicsObject ympyra = new PhysicsObject(leveys, korkeus, Shape.Circle);
        ympyra.Position = RandomGen.NextVector(pieninx, pieniny, isoinx, isoinY);
        peli.Add(ympyra);
    }
}


