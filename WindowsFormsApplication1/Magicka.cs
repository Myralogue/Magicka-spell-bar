using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

/* 
 * 
 * 
 * 
 * 
 * Use at your own Risk, we take no responsibility for problems caused by this program or any other program using our code.
 * This code is meant to be used to check your OWN key input, NOT the inputs of SOMEONE else.
 * Do not use this code for anything else then its purpose (Magicka spell log).
 * For your own safety make sure the "pause" button is activated while typing in your Password/Login.
 * Although robust enough for general use, adventures into the esoteric periphery may reveal unexpected quirks, be cautious..
 * 
 * For questions or support contact:steamgear@web.de
 * Made by: Martijn, René and SteamGear.
 * 
 * Feel free to change or use to sourcecode for your own programs
 * but we dont take ANY responsability if you do so.
 * 
 * 
 * 
 * 
 */

namespace WindowsFormsApplication1
{
    public partial class Magicka : Form
    {
        public Magicka()
        {
            InitializeComponent();
        }
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);
        //global variables
        string keyBuffer = string.Empty;
        List<String> aKeyBuffer = new List<String>();
        Dictionary<String, int> dict2 = new Dictionary<String, int>();
        Dictionary<String, int> dictCheck = new Dictionary<String, int>();
        string[] skills = new string[] { "Q", "W", "E", "R", "A", "S", "D", "F" };
        string[] specialkeys = new string[] { "shift", "lmb", "rmb", "mmb" };
        string currentchain = "";

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {

                int x = GetAsyncKeyState(i);
                if (x != dict2[Enum.GetName(typeof(Keys), i) + " "])
                {

                    if ((x == 1) || (x == Int16.MinValue)) //Use constants (0x8000 and -32768 is Int16.MaxValue)
                    {
                        if (dictCheck[Enum.GetName(typeof(Keys), i) + " "] > 35)
                        {
                            aKeyBuffer.Add(Enum.GetName(typeof(Keys), i) + " ");
                            dictCheck[Enum.GetName(typeof(Keys), i) + " "] = 0;
                        }
                        else
                        {
                            dictCheck[Enum.GetName(typeof(Keys), i) + " "] = 0;
                        }
                    }
                }
                dict2[Enum.GetName(typeof(Keys), i) + " "] = x;
                dictCheck[Enum.GetName(typeof(Keys), i) + " "] += 10;
            }

            if (aKeyBuffer.Count() > 0 && !aKeyBuffer[0].Equals(""))
            {
                keyBuffer = aKeyBuffer[0];
            }
            if (keyBuffer != "" && aKeyBuffer.Count() > 0)
            {
                // replacing of the keys we need             
                keyBuffer = keyBuffer.Replace("Space", "space");
                keyBuffer = keyBuffer.Replace("LShiftKey", "shift");
                keyBuffer = keyBuffer.Replace("LButton", "lmb");
                keyBuffer = keyBuffer.Replace("RButton", "rmb");
                keyBuffer = keyBuffer.Replace("MButton", "mmb");
                keyBuffer = keyBuffer.Replace("Z", "");
                keyBuffer = keyBuffer.Replace("X", "");
                keyBuffer = keyBuffer.Replace(" ", "");

                aKeyBuffer.Clear();
                if (skills.Any(keyBuffer.Contains) && keyBuffer.Length == 1)
                {
                    currentchain += keyBuffer;
                    specials();
                }
                else if (specialkeys.Any(keyBuffer.Contains))
                {
                    actionkeys();
                }
                else if (keyBuffer == "space")
                {
                    spells();
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {
                dict2[Enum.GetName(typeof(Keys), i) + " "] = 0;
                dictCheck[Enum.GetName(typeof(Keys), i) + " "] = 0;
            }
        }


        public void specials()
        {
            string E = "E";
            for (int C = 0; C < currentchain.Length; C++)
            {
                for (int R = 0; R < currentchain.Length; R++)
                {
                    if (currentchain[C] == 'A' && currentchain[R] == 'D')
                    {
                        currentchain = GetOrder(currentchain, "A", "D");
                        break;
                    }
                    if (currentchain[C] == 'E' && (currentchain.Length - currentchain.Replace(E, "").Length) / E.Length > 1)
                    {
                        currentchain = currentchain.Replace("E", "");
                        break;
                    }
                    if (currentchain[C] == 'W' && currentchain[R] == 'S')
                    {
                        currentchain = GetOrder(currentchain, "W", "S");
                        break;
                    }
                    if (currentchain[C] == 'F' && currentchain[R] == 'R')
                    {
                        currentchain = GetOrder(currentchain, "F", "R");
                        break;
                    }
                    if (currentchain[C] == 'A' && currentchain[R] == 'Q')
                    {
                        currentchain = GetOrder(currentchain, "A", "Q");
                        break;
                    }
                    if (currentchain[C] == 'Q' && currentchain[R] == 'F')
                    {
                        currentchain = GetOrder(currentchain, "Q", "F");
                        currentchain = currentchain + "Z";
                        break;
                    }
                    if (currentchain[C] == 'Q' && currentchain[R] == 'R')
                    {
                        currentchain = GetOrder(currentchain, "Q", "R");
                        currentchain = currentchain + "X";
                        break;
                    }
                    if (currentchain[C] == 'X' && currentchain[R] == 'F')
                    {
                        currentchain = GetOrder(currentchain, "X", "F");
                        currentchain = currentchain + "Q";
                        break;
                    }
                    if (currentchain[C] == 'Z' && currentchain[R] == 'R')
                    {
                        currentchain = GetOrder(currentchain, "Z", "R");
                        currentchain = currentchain + "Q";
                        break;
                    }
                }
            }
            for (int i = currentchain.Length; i > 5; i--)
            {
                currentchain = currentchain.Remove((currentchain.Length - 1), 1);
            }
            writecurrent();
        }
        public void actionkeys()
        {
            string actionkey = "";
            if (currentchain.Length != 0)
            {
                if ((GetAsyncKeyState(160) == Int16.MinValue))
                {
                    actionkey = "shift" + keyBuffer;
                    switch (actionkey)
                    {
                        case "shiftlmb":
                            NextSpellType.BackgroundImage = Properties.Resources.sword1;
                            break;
                        case "shiftrmb":
                            NextSpellType.BackgroundImage = Properties.Resources.aoe;
                            break;
                        case "shiftmmb":
                            NextSpellType.BackgroundImage = Properties.Resources.self;
                            break;
                    }
                    moverow();
                }
                else if (keyBuffer != "lmb")
                {
                    actionkey = keyBuffer;
                    switch (actionkey)
                    {
                        case "rmb":
                            NextSpellType.BackgroundImage = Properties.Resources.normal1;
                            break;
                        case "mmb":
                            NextSpellType.BackgroundImage = Properties.Resources.self;
                            break;
                    }
                    moverow();
                }
            }
        }
        public void chain()
        {
            if (skills.Any(keyBuffer.Contains) && currentchain.Length != 0 && keyBuffer.Length == 1)
            {
                moverow();
            }
        }
        public string RebuildString(string current, String First, String Second)
        {
            string KeepString = current;
            int firstCount = 0;
            int secondCount = 0;

            for (int i = KeepString.Length; i > 0; i--)
            {
                if (First == KeepString.Substring((i - 1), (1)) && firstCount == 0)
                {
                    KeepString = KeepString.Remove((i - 1), 1);
                    firstCount++;
                }
                else if (Second == KeepString.Substring((i - 1), (1)) && secondCount == 0)
                {
                    KeepString = KeepString.Remove((i - 1), 1);
                    secondCount++;
                }
            }
            current = KeepString;
            return current;
        }

        public String GetOrder(String current, String char1, String char2)
        {

            int index = current.IndexOf(char1);
            int index2 = current.IndexOf(char2);
            String first = "";
            String second = "";
            if (index > index2)
            {
                first = char2;
                second = char1;
            }
            else
            {
                first = char1;
                second = char2;
            }

            currentchain = RebuildString(currentchain, first, second);
            return currentchain;
        }
        public void moverow()
        {
            //this void moves all the icons up 1 row.
            for (int i = 5; i > 1; i--)
            {

                this.Controls["r" + i.ToString() + "O1"].BackgroundImage = this.Controls["r" + (i - 1).ToString() + "O1"].BackgroundImage;
                this.Controls["r" + i.ToString() + "O2"].BackgroundImage = this.Controls["r" + (i - 1).ToString() + "O2"].BackgroundImage;
                this.Controls["r" + i.ToString() + "O3"].BackgroundImage = this.Controls["r" + (i - 1).ToString() + "O3"].BackgroundImage;
                this.Controls["r" + i.ToString() + "O4"].BackgroundImage = this.Controls["r" + (i - 1).ToString() + "O4"].BackgroundImage;
                this.Controls["r" + i.ToString() + "O5"].BackgroundImage = this.Controls["r" + (i - 1).ToString() + "O5"].BackgroundImage;
                this.Controls["r" + i.ToString() + "O6"].BackgroundImage = this.Controls["r" + (i - 1).ToString() + "O6"].BackgroundImage;


            }
            for (int i = 5; i > 0; i--)
            {
                this.Controls["r1O" + i.ToString()].BackgroundImage = this.Controls["C" + i.ToString()].BackgroundImage;
                this.Controls["C" + i.ToString()].BackgroundImage = null;

            }
            //attack type icons
            r1O6.BackgroundImage = NextSpellType.BackgroundImage;
            NextSpellType.BackgroundImage = null;
            currentchain = "";

        }
        public void writecurrent()
        {
            string writing = "";
            C1.BackgroundImage = null;
            C2.BackgroundImage = null;
            C3.BackgroundImage = null;
            C4.BackgroundImage = null;
            C5.BackgroundImage = null;

            for (int i = currentchain.Length; i > 0; i--)
            {
                writing = currentchain.Substring((i - 1), (1));
                switch (writing)
                {
                    //qwerasdfzx
                    case "Q":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._32px_Element_water;
                        break;
                    case "W":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._33px_Element_life;
                        break;
                    case "E":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._33px_Element_shield;
                        break;
                    case "R":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._32px_Element_cold;
                        break;
                    case "A":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._32px_Element_lightning;
                        break;
                    case "S":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._33px_Element_arcane;
                        break;
                    case "D":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._32px_Element_earth;
                        break;
                    case "F":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._32px_Element_fire;
                        break;
                    case "Z":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._33px_Element_steam;
                        break;
                    case "X":
                        this.Controls["C" + i.ToString()].BackgroundImage = Properties.Resources._33px_Element_ice;
                        break;

                }


            }

        }
        public void spells()
        {
            int i = 0;

            switch (currentchain)
            {
                case "ASA": //Teleport
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_teleport1;
                    break;
                case "WA": //Revive
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_revive;
                    break;
                case "ASF": //Haste
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_haste;
                    break;
                case "QDW": //Grease
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_grease;
                    break;
                case "QZ": //Rain
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_rain;
                    break;
                case "SE": //Nullify
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_nullify;
                    break;
                case "ZASA": //Thunderbolt
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_thunderbolt;
                    break;
                case "DZQZ": //Tornado
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_tornado;
                    break;
                case "ZFZFZ": //Conflagration
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_conflagration;
                    break;
                case "RE": //Timewarp
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_time_warp;
                    break;
                case "RXR": //Blizzard
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_blizzard;
                    break;
                case "ZZASA": //Thunderstorm
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_thunderstorm;
                    break;
                case "WAF": //Summon Phoenix
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_summonphoenix;
                    break;
                case "XDSR": //Raise Dead
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_raisedead;
                    break;
                case "RSE": //Fear
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_fear;
                    break;
                case "WED": //Charm
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_charm;
                    break;
                case "SRXRS": //Summon Death
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_summondeath;
                    break;
                case "SEZS": //Invisibility
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_invisibility;
                    break;
                case "SEDZS": //Summon Elemental
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_elemental;
                    break;
                case "SZAES": //Corporealize
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_corporealize;
                    break;
                case "XSXEX": //Vortex
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_vortex;
                    break;
                case "SEA": //Confuse
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_Confuse;
                    break;
                //---------------- (Expension spells)
                case "FDZDF": //Meteor Shower
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Magick_meteorshower_4;
                    break;
                case "AAFW": //Crash To Desktop
                    NextSpellType.BackgroundImage = Properties.Resources._160px_CTD;
                    break;
                case "ZDWFF": //Napalm (no pic)
                    break;
                case "ZAE": //Portal
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Portal;
                    break;
                case "DS": //Tractor Pull
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Tractor_Pull;
                    break;
                case "FZS": //Propp's Party Plasma
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Popps_Party_Plasma;
                    break;
                case "ZSZ": //Levitation
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Levitation;
                    break;
                case "AAA": //Chain Lightning
                    NextSpellType.BackgroundImage = Properties.Resources._160px_Chain_lightning;
                    break;
                default: //None of the above
                    i = 1;
                    break;
            }
            if (i == 0) { moverow(); }
        }

        private void Settingsbt_Click(object sender, EventArgs e)
        {
            Focusbt.Focus();
            if (SettingsPanel.Visible == false)
            {
                SettingsPanel.Visible = true;
                Settingsbt.BackColor = Color.LightSteelBlue;
            }
            else
            {
                SettingsPanel.Visible = false;
                Settingsbt.BackColor = Color.White;
            }

        }

        private void Borderbt_Click(object sender, EventArgs e)
        {
            if (Magicka.ActiveForm.FormBorderStyle == FormBorderStyle.FixedSingle)
            {
                Magicka.ActiveForm.FormBorderStyle = FormBorderStyle.None;
                Borderbt.BackColor = Color.LightSteelBlue;
            }
            else
            {
                Magicka.ActiveForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                Borderbt.BackColor = Color.White;
            }
            Focusbt.Focus();

        }

        private void Pausebt_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Enabled = false;
                Pausebt.BackColor = Color.LightSteelBlue;
            }
            else
            {
                Pausebt.BackColor = Color.White;
                timer1.Enabled = true;

            }
            Focusbt.Focus();
        }

        private void Topbt_Click(object sender, EventArgs e)
        {
            if (this.TopMost == false)
            {
                this.TopMost = true;
                Topbt.BackColor = Color.LightSteelBlue;
            }
            else
            {
                this.TopMost = false;
                Topbt.BackColor = Color.White;
            }
            Focusbt.Focus();
        }

        private void Closebt_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }


}

