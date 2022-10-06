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
        BoundingRectangle alaosa = new BoundingRectangle(new Vector(Level.Left, 0), Level.BoundingRect.BottomRight);
        BoundingRectangle ylaosa = new BoundingRectangle(Level.BoundingRect.TopLeft, new Vector(Level.Right, 0));

        for (int i = 0; i < 15; i++) // vihaiset asiakkat
        {
            LuoSatunaisetAsiakkaat(this, alaosa,60);

        }
        PhysicsObject pelaaja =  LuoSatunaisetAsiakkaat(this,ylaosa,50);
        pelaaja.Image = LoadImage("pelaaja");
        pelaaja.Color = Color.Red;

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Level.Background.Color = Color.Black;
        Level.CreateBorders();

    }
    /// <summary>
    /// Aliohjelma luo vihamieliset asiakkaat
    /// </summary>
    /// <param name="peli">mihin peliin ympyrä luodaan</param>
    /// <param name="umpyra">minkä ymppyrän sisälle luodaan eli paikka</param>
    /// <param name="vauhti">kolmion vauhti alussa</param>
    public static PhysicsObject LuoSatunaisetAsiakkaat(PhysicsGame peli,BoundingRectangle umpyra,double vauhti)
    {
        double leveys = RandomGen.NextDouble(40, 40);
        double korkeus = RandomGen.NextDouble(40, 40);  
        Vector liike = RandomGen.NextVector(0, vauhti);
        PhysicsObject ympyra = new PhysicsObject(leveys, korkeus, Shape.Circle);
        ympyra.Hit(liike);
        peli.Add(ympyra);
        return ympyra;
    }
}


