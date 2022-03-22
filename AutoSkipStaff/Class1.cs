using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoSkipStaff
{
    public class MainBehaviour : ModBehaviour
    {
        int originalSSpeed = 0;
        int originalSpeed = 0;
        bool hasSkipped = false;
        public override void OnActivate()
        {
            TimeOfDay.OnHourPassed += HourPassed;
        }

        void Update()
        {

        }

        private void HourPassed(object sender, EventArgs e)
        {
            DevConsole.Console.Log("Hour passed");

            bool anyEmployeeInOffice = false;
            bool anyStaffInOffice = false;

            foreach (Actor actor in GameSettings.Instance.sActorManager.AllActors())
            {
                if (actor.AItype == AI.AIType.Employee)
                {
                    if (TimeOfDay.Instance.Hour >= actor.StaffOn-1 && TimeOfDay.Instance.Hour < actor.StaffOff) anyEmployeeInOffice = true;
                    if(actor.CurrentState(false) != "At home" && actor.CurrentState(false) != "Sick" && actor.CurrentState(false) != "Vacation") {
                        DevConsole.Console.Log(actor.employee.Name + " in office (" + actor.CurrentState(false) + ")");
                        anyEmployeeInOffice = true;
                    }
                }else
                {
                    if (actor.CurrentState(false) != "At home")
                    {
                        anyStaffInOffice = true;
                    }
                }
            }

            DevConsole.Console.Log("Any employee in office: " + anyEmployeeInOffice);
            DevConsole.Console.Log("Any staff in office: " + anyStaffInOffice);

            if (!anyEmployeeInOffice && anyStaffInOffice)
            {
                DevConsole.Console.Log("Skipping time...");
                DevConsole.Console.Log("GameSpeed: " + GameSettings.GameSpeed);

                if (Options.SecondSpeed != 200) originalSSpeed = Options.SecondSpeed;

                var field = typeof(Options).GetField("_secondSpeed",
                            BindingFlags.Static |
                            BindingFlags.NonPublic);

                field.SetValue(null, 200);

                if (!hasSkipped) originalSpeed = HUD.Instance.GameSpeed;
                HUD.UpdateSpeeds();
                HUD.Instance.GameSpeed = 2;
                hasSkipped = true;

                DevConsole.Console.Log("GameSpeed: " + GameSettings.GameSpeed);
            }
            else if(hasSkipped)
            {
                hasSkipped = false;
                if (originalSSpeed != 0) Options.SecondSpeed = originalSSpeed;
                HUD.UpdateSpeeds();
                if(originalSpeed != 0) HUD.Instance.GameSpeed = originalSpeed;
            }
        }

        public override void OnDeactivate()
        {

        }
    }

    internal class SomeModMeta : ModMeta
    {
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {

        }

        public override string Name => "AutoSkipStaff";
        public static bool GiveMeFreedom = true;
    }
}
