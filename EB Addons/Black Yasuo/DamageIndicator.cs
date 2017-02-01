﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;

using Line = EloBuddy.SDK.Rendering.Line;

namespace BlackYasuo
{
    public static class DamageIndicator
    {
        //Offsets
        private const float YOff = 9.8f;
        private const float XOff = 0;
        private const float Width = 107;
        private const float Thick = 9.82f;
        //Offsets
        private static Font _Font, _Font2;

        public static void Load()
        {
            Drawing.OnEndScene += Drawing_OnEndScene;

            _Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 16,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.TrueType,
                    Quality = FontQuality.ClearType
                });

            _Font2 = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 11,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.TrueType,
                    Quality = FontQuality.ClearType
                });
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(e => e.IsValid && e.IsHPBarRendered && e.TotalShieldHealth() > 10)
                )
            {
                var damage = enemy.GetTotalDamage();
                if (MyMenu.DrawMenu.GetCheckBoxValue("damageColorBox"))
                {
                    //Drawing Line Over Enemies Helth bar
                    var dmgPer = (enemy.TotalShieldHealth() - damage > 0 ? enemy.TotalShieldHealth() - damage : 0) /
                                 enemy.TotalShieldMaxHealth();
                    var currentHPPer = enemy.TotalShieldHealth() / enemy.TotalShieldMaxHealth();
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width),
                        (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1,
                        (int)enemy.HPBarPosition.Y + YOff);

                    var colour = MyMenu.DamageIndicatorColor.CurrentColor;
                    Line.DrawLine(colour, Thick, initPoint, endPoint);
                }

                if (MyMenu.DrawMenu.GetCheckBoxValue("statDraw"))
                {
                    //Statistics
                    var posXStat = (int)enemy.HPBarPosition[0] - 46;
                    var posYStat = (int)enemy.HPBarPosition[1] + 12;
                    var mathStat = " - " + Math.Round(damage) + " / " +
                                   Math.Round(enemy.Health - damage);
                    _Font2.DrawText(null, mathStat, posXStat, posYStat, MyMenu.DamageIndicatorColor.CurrentColor.ToSharpDX());
                }

                if (MyMenu.DrawMenu.GetCheckBoxValue("perDraw"))
                {
                    //Percent
                    var posXPer = (int)enemy.HPBarPosition[0] - 28;
                    var posYPer = (int)enemy.HPBarPosition[1];
                    _Font.DrawText(null, string.Concat(Math.Ceiling((int)damage / enemy.TotalShieldHealth() * 100), "%"),
                        posXPer, posYPer, MyMenu.DamageIndicatorColor.CurrentColor.ToSharpDX());
                }
            }
        }
    }
}
