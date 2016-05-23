using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Medvedya.SpriteDeformerTools
{


    [ExecuteInEditMode]
    [AddComponentMenu("Sprite Deformer/Sprite deformer Animation")]
    public class SpriteDeformerAnimation :SpriteDeformerWithMaterialPropertyBlock,ISerializationCallbackReceiver
    {
        [System.Serializable]
        public struct PointInfo
        {
            public Color color;
            public Vector3 offset;
            public Vector2 position;
        }
        /// <summary>
        /// Not animated points. If you modife point from code add this point there.
        /// </summary>
        public List<SpritePoint> notAnimatedPoints = new List<SpritePoint>();
        public override void AddPoint(SpritePoint point, bool autoOffset = false)
        {
            if (base.points.Count >= 256)
            {
                Debug.Log("You can't have more any than 256 points");
                return;
            }
            base.AddPoint(point, autoOffset);
            for (int i = 0; i < 256; i++)
            {
                if (!busyPoints[i])
                {
                    busyPoints[i] = true;
                    animationPoints[i] = point;
                    PointInfo pi;
                    pi.position = point.spritePosition;
                    pi.color = point.color;
                    pi.offset = point.offset;
                    setValueByIndex(i, pi);
                    break;
                }
            }
            
        }
        public override void RemovePoint(SpritePoint p)
        {
            
            int index = -1;
            for (int i = 0; i < 256; i++)
			{
			    if(p == animationPoints[i])
                {
                    index = i;
                    break;
                }
			}
            if(index != -1)
            busyPoints[index] = false;
            
            base.RemovePoint(p);
        }
        protected override void Update()
        {
            if (this.meshRender.isVisible)
            {
                for (int i = 0; i < 256; i++)
                {
                    if (busyPoints[i] && !notAnimatedPoints.Contains(animationPoints[i]))
                    {
                        PointInfo pi = getValueByIndex(i);
                        if (pi.color != animationPoints[i].color)
                        {
                            dirty_color = true;
                            animationPoints[i].color = pi.color;
                        }
                        if (pi.offset != animationPoints[i].offset)
                        {
                            dirty_offset = true;
                            animationPoints[i].offset = pi.offset;
                        }
                    }
                }
            }
            base.Update();
        }

        [SerializeField]
        private bool[] busyPoints = new bool[256];
        [SerializeField]
        public SpritePoint[] animationPoints = new SpritePoint[256];
        [SerializeField]
        private int[] serelizateAnimationPoints = new int[256];
        [SerializeField]
        private PointInfo v000 = new PointInfo(), v001 = new PointInfo(), v002 = new PointInfo(), v003 = new PointInfo(), v004 = new PointInfo(), v005 = new PointInfo(), v006 = new PointInfo(), v007 = new PointInfo(), v008 = new PointInfo(), v009 = new PointInfo(), v010 = new PointInfo(), v011 = new PointInfo(), v012 = new PointInfo(), v013 = new PointInfo(), v014 = new PointInfo(), v015 = new PointInfo(), v016 = new PointInfo(), v017 = new PointInfo(), v018 = new PointInfo(), v019 = new PointInfo(), v020 = new PointInfo(), v021 = new PointInfo(), v022 = new PointInfo(), v023 = new PointInfo(), v024 = new PointInfo(), v025 = new PointInfo(), v026 = new PointInfo(), v027 = new PointInfo(), v028 = new PointInfo(), v029 = new PointInfo(), v030 = new PointInfo(), v031 = new PointInfo(), v032 = new PointInfo(), v033 = new PointInfo(), v034 = new PointInfo(), v035 = new PointInfo(), v036 = new PointInfo(), v037 = new PointInfo(), v038 = new PointInfo(), v039 = new PointInfo(), v040 = new PointInfo(), v041 = new PointInfo(), v042 = new PointInfo(), v043 = new PointInfo(), v044 = new PointInfo(), v045 = new PointInfo(), v046 = new PointInfo(), v047 = new PointInfo(), v048 = new PointInfo(), v049 = new PointInfo(), v050 = new PointInfo(), v051 = new PointInfo(), v052 = new PointInfo(), v053 = new PointInfo(), v054 = new PointInfo(), v055 = new PointInfo(), v056 = new PointInfo(), v057 = new PointInfo(), v058 = new PointInfo(), v059 = new PointInfo(), v060 = new PointInfo(), v061 = new PointInfo(), v062 = new PointInfo(), v063 = new PointInfo(), v064 = new PointInfo(), v065 = new PointInfo(), v066 = new PointInfo(), v067 = new PointInfo(), v068 = new PointInfo(), v069 = new PointInfo(), v070 = new PointInfo(), v071 = new PointInfo(), v072 = new PointInfo(), v073 = new PointInfo(), v074 = new PointInfo(), v075 = new PointInfo(), v076 = new PointInfo(), v077 = new PointInfo(), v078 = new PointInfo(), v079 = new PointInfo(), v080 = new PointInfo(), v081 = new PointInfo(), v082 = new PointInfo(), v083 = new PointInfo(), v084 = new PointInfo(), v085 = new PointInfo(), v086 = new PointInfo(), v087 = new PointInfo(), v088 = new PointInfo(), v089 = new PointInfo(), v090 = new PointInfo(), v091 = new PointInfo(), v092 = new PointInfo(), v093 = new PointInfo(), v094 = new PointInfo(), v095 = new PointInfo(), v096 = new PointInfo(), v097 = new PointInfo(), v098 = new PointInfo(), v099 = new PointInfo(), v100 = new PointInfo(), v101 = new PointInfo(), v102 = new PointInfo(), v103 = new PointInfo(), v104 = new PointInfo(), v105 = new PointInfo(), v106 = new PointInfo(), v107 = new PointInfo(), v108 = new PointInfo(), v109 = new PointInfo(), v110 = new PointInfo(), v111 = new PointInfo(), v112 = new PointInfo(), v113 = new PointInfo(), v114 = new PointInfo(), v115 = new PointInfo(), v116 = new PointInfo(), v117 = new PointInfo(), v118 = new PointInfo(), v119 = new PointInfo(), v120 = new PointInfo(), v121 = new PointInfo(), v122 = new PointInfo(), v123 = new PointInfo(), v124 = new PointInfo(), v125 = new PointInfo(), v126 = new PointInfo(), v127 = new PointInfo(), v128 = new PointInfo(), v129 = new PointInfo(), v130 = new PointInfo(), v131 = new PointInfo(), v132 = new PointInfo(), v133 = new PointInfo(), v134 = new PointInfo(), v135 = new PointInfo(), v136 = new PointInfo(), v137 = new PointInfo(), v138 = new PointInfo(), v139 = new PointInfo(), v140 = new PointInfo(), v141 = new PointInfo(), v142 = new PointInfo(), v143 = new PointInfo(), v144 = new PointInfo(), v145 = new PointInfo(), v146 = new PointInfo(), v147 = new PointInfo(), v148 = new PointInfo(), v149 = new PointInfo(), v150 = new PointInfo(), v151 = new PointInfo(), v152 = new PointInfo(), v153 = new PointInfo(), v154 = new PointInfo(), v155 = new PointInfo(), v156 = new PointInfo(), v157 = new PointInfo(), v158 = new PointInfo(), v159 = new PointInfo(), v160 = new PointInfo(), v161 = new PointInfo(), v162 = new PointInfo(), v163 = new PointInfo(), v164 = new PointInfo(), v165 = new PointInfo(), v166 = new PointInfo(), v167 = new PointInfo(), v168 = new PointInfo(), v169 = new PointInfo(), v170 = new PointInfo(), v171 = new PointInfo(), v172 = new PointInfo(), v173 = new PointInfo(), v174 = new PointInfo(), v175 = new PointInfo(), v176 = new PointInfo(), v177 = new PointInfo(), v178 = new PointInfo(), v179 = new PointInfo(), v180 = new PointInfo(), v181 = new PointInfo(), v182 = new PointInfo(), v183 = new PointInfo(), v184 = new PointInfo(), v185 = new PointInfo(), v186 = new PointInfo(), v187 = new PointInfo(), v188 = new PointInfo(), v189 = new PointInfo(), v190 = new PointInfo(), v191 = new PointInfo(), v192 = new PointInfo(), v193 = new PointInfo(), v194 = new PointInfo(), v195 = new PointInfo(), v196 = new PointInfo(), v197 = new PointInfo(), v198 = new PointInfo(), v199 = new PointInfo(), v200 = new PointInfo(), v201 = new PointInfo(), v202 = new PointInfo(), v203 = new PointInfo(), v204 = new PointInfo(), v205 = new PointInfo(), v206 = new PointInfo(), v207 = new PointInfo(), v208 = new PointInfo(), v209 = new PointInfo(), v210 = new PointInfo(), v211 = new PointInfo(), v212 = new PointInfo(), v213 = new PointInfo(), v214 = new PointInfo(), v215 = new PointInfo(), v216 = new PointInfo(), v217 = new PointInfo(), v218 = new PointInfo(), v219 = new PointInfo(), v220 = new PointInfo(), v221 = new PointInfo(), v222 = new PointInfo(), v223 = new PointInfo(), v224 = new PointInfo(), v225 = new PointInfo(), v226 = new PointInfo(), v227 = new PointInfo(), v228 = new PointInfo(), v229 = new PointInfo(), v230 = new PointInfo(), v231 = new PointInfo(), v232 = new PointInfo(), v233 = new PointInfo(), v234 = new PointInfo(), v235 = new PointInfo(), v236 = new PointInfo(), v237 = new PointInfo(), v238 = new PointInfo(), v239 = new PointInfo(), v240 = new PointInfo(), v241 = new PointInfo(), v242 = new PointInfo(), v243 = new PointInfo(), v244 = new PointInfo(), v245 = new PointInfo(), v246 = new PointInfo(), v247 = new PointInfo(), v248 = new PointInfo(), v249 = new PointInfo(), v250 = new PointInfo(), v251 = new PointInfo(), v252 = new PointInfo(), v253 = new PointInfo(), v254 = new PointInfo(), v255 = new PointInfo();
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }


        public PointInfo getValueByIndex(int i)
        {
            switch (i)
            {
                case 0: return v000;
                case 1: return v001;
                case 2: return v002;
                case 3: return v003;
                case 4: return v004;
                case 5: return v005;
                case 6: return v006;
                case 7: return v007;
                case 8: return v008;
                case 9: return v009;
                case 10: return v010;
                case 11: return v011;
                case 12: return v012;
                case 13: return v013;
                case 14: return v014;
                case 15: return v015;
                case 16: return v016;
                case 17: return v017;
                case 18: return v018;
                case 19: return v019;
                case 20: return v020;
                case 21: return v021;
                case 22: return v022;
                case 23: return v023;
                case 24: return v024;
                case 25: return v025;
                case 26: return v026;
                case 27: return v027;
                case 28: return v028;
                case 29: return v029;
                case 30: return v030;
                case 31: return v031;
                case 32: return v032;
                case 33: return v033;
                case 34: return v034;
                case 35: return v035;
                case 36: return v036;
                case 37: return v037;
                case 38: return v038;
                case 39: return v039;
                case 40: return v040;
                case 41: return v041;
                case 42: return v042;
                case 43: return v043;
                case 44: return v044;
                case 45: return v045;
                case 46: return v046;
                case 47: return v047;
                case 48: return v048;
                case 49: return v049;
                case 50: return v050;
                case 51: return v051;
                case 52: return v052;
                case 53: return v053;
                case 54: return v054;
                case 55: return v055;
                case 56: return v056;
                case 57: return v057;
                case 58: return v058;
                case 59: return v059;
                case 60: return v060;
                case 61: return v061;
                case 62: return v062;
                case 63: return v063;
                case 64: return v064;
                case 65: return v065;
                case 66: return v066;
                case 67: return v067;
                case 68: return v068;
                case 69: return v069;
                case 70: return v070;
                case 71: return v071;
                case 72: return v072;
                case 73: return v073;
                case 74: return v074;
                case 75: return v075;
                case 76: return v076;
                case 77: return v077;
                case 78: return v078;
                case 79: return v079;
                case 80: return v080;
                case 81: return v081;
                case 82: return v082;
                case 83: return v083;
                case 84: return v084;
                case 85: return v085;
                case 86: return v086;
                case 87: return v087;
                case 88: return v088;
                case 89: return v089;
                case 90: return v090;
                case 91: return v091;
                case 92: return v092;
                case 93: return v093;
                case 94: return v094;
                case 95: return v095;
                case 96: return v096;
                case 97: return v097;
                case 98: return v098;
                case 99: return v099;
                case 100: return v100;
                case 101: return v101;
                case 102: return v102;
                case 103: return v103;
                case 104: return v104;
                case 105: return v105;
                case 106: return v106;
                case 107: return v107;
                case 108: return v108;
                case 109: return v109;
                case 110: return v110;
                case 111: return v111;
                case 112: return v112;
                case 113: return v113;
                case 114: return v114;
                case 115: return v115;
                case 116: return v116;
                case 117: return v117;
                case 118: return v118;
                case 119: return v119;
                case 120: return v120;
                case 121: return v121;
                case 122: return v122;
                case 123: return v123;
                case 124: return v124;
                case 125: return v125;
                case 126: return v126;
                case 127: return v127;
                case 128: return v128;
                case 129: return v129;
                case 130: return v130;
                case 131: return v131;
                case 132: return v132;
                case 133: return v133;
                case 134: return v134;
                case 135: return v135;
                case 136: return v136;
                case 137: return v137;
                case 138: return v138;
                case 139: return v139;
                case 140: return v140;
                case 141: return v141;
                case 142: return v142;
                case 143: return v143;
                case 144: return v144;
                case 145: return v145;
                case 146: return v146;
                case 147: return v147;
                case 148: return v148;
                case 149: return v149;
                case 150: return v150;
                case 151: return v151;
                case 152: return v152;
                case 153: return v153;
                case 154: return v154;
                case 155: return v155;
                case 156: return v156;
                case 157: return v157;
                case 158: return v158;
                case 159: return v159;
                case 160: return v160;
                case 161: return v161;
                case 162: return v162;
                case 163: return v163;
                case 164: return v164;
                case 165: return v165;
                case 166: return v166;
                case 167: return v167;
                case 168: return v168;
                case 169: return v169;
                case 170: return v170;
                case 171: return v171;
                case 172: return v172;
                case 173: return v173;
                case 174: return v174;
                case 175: return v175;
                case 176: return v176;
                case 177: return v177;
                case 178: return v178;
                case 179: return v179;
                case 180: return v180;
                case 181: return v181;
                case 182: return v182;
                case 183: return v183;
                case 184: return v184;
                case 185: return v185;
                case 186: return v186;
                case 187: return v187;
                case 188: return v188;
                case 189: return v189;
                case 190: return v190;
                case 191: return v191;
                case 192: return v192;
                case 193: return v193;
                case 194: return v194;
                case 195: return v195;
                case 196: return v196;
                case 197: return v197;
                case 198: return v198;
                case 199: return v199;
                case 200: return v200;
                case 201: return v201;
                case 202: return v202;
                case 203: return v203;
                case 204: return v204;
                case 205: return v205;
                case 206: return v206;
                case 207: return v207;
                case 208: return v208;
                case 209: return v209;
                case 210: return v210;
                case 211: return v211;
                case 212: return v212;
                case 213: return v213;
                case 214: return v214;
                case 215: return v215;
                case 216: return v216;
                case 217: return v217;
                case 218: return v218;
                case 219: return v219;
                case 220: return v220;
                case 221: return v221;
                case 222: return v222;
                case 223: return v223;
                case 224: return v224;
                case 225: return v225;
                case 226: return v226;
                case 227: return v227;
                case 228: return v228;
                case 229: return v229;
                case 230: return v230;
                case 231: return v231;
                case 232: return v232;
                case 233: return v233;
                case 234: return v234;
                case 235: return v235;
                case 236: return v236;
                case 237: return v237;
                case 238: return v238;
                case 239: return v239;
                case 240: return v240;
                case 241: return v241;
                case 242: return v242;
                case 243: return v243;
                case 244: return v244;
                case 245: return v245;
                case 246: return v246;
                case 247: return v247;
                case 248: return v248;
                case 249: return v249;
                case 250: return v250;
                case 251: return v251;
                case 252: return v252;
                case 253: return v253;
                case 254: return v254;
                case 255: return v255;
                default: return new PointInfo();
            }
            //return null;
        }
        public void setValueByIndex(int i, PointInfo value)
        {
            switch (i)
            {
                case 0: { v000 = value; return; }
                case 1: { v001 = value; return; }
                case 2: { v002 = value; return; }
                case 3: { v003 = value; return; }
                case 4: { v004 = value; return; }
                case 5: { v005 = value; return; }
                case 6: { v006 = value; return; }
                case 7: { v007 = value; return; }
                case 8: { v008 = value; return; }
                case 9: { v009 = value; return; }
                case 10: { v010 = value; return; }
                case 11: { v011 = value; return; }
                case 12: { v012 = value; return; }
                case 13: { v013 = value; return; }
                case 14: { v014 = value; return; }
                case 15: { v015 = value; return; }
                case 16: { v016 = value; return; }
                case 17: { v017 = value; return; }
                case 18: { v018 = value; return; }
                case 19: { v019 = value; return; }
                case 20: { v020 = value; return; }
                case 21: { v021 = value; return; }
                case 22: { v022 = value; return; }
                case 23: { v023 = value; return; }
                case 24: { v024 = value; return; }
                case 25: { v025 = value; return; }
                case 26: { v026 = value; return; }
                case 27: { v027 = value; return; }
                case 28: { v028 = value; return; }
                case 29: { v029 = value; return; }
                case 30: { v030 = value; return; }
                case 31: { v031 = value; return; }
                case 32: { v032 = value; return; }
                case 33: { v033 = value; return; }
                case 34: { v034 = value; return; }
                case 35: { v035 = value; return; }
                case 36: { v036 = value; return; }
                case 37: { v037 = value; return; }
                case 38: { v038 = value; return; }
                case 39: { v039 = value; return; }
                case 40: { v040 = value; return; }
                case 41: { v041 = value; return; }
                case 42: { v042 = value; return; }
                case 43: { v043 = value; return; }
                case 44: { v044 = value; return; }
                case 45: { v045 = value; return; }
                case 46: { v046 = value; return; }
                case 47: { v047 = value; return; }
                case 48: { v048 = value; return; }
                case 49: { v049 = value; return; }
                case 50: { v050 = value; return; }
                case 51: { v051 = value; return; }
                case 52: { v052 = value; return; }
                case 53: { v053 = value; return; }
                case 54: { v054 = value; return; }
                case 55: { v055 = value; return; }
                case 56: { v056 = value; return; }
                case 57: { v057 = value; return; }
                case 58: { v058 = value; return; }
                case 59: { v059 = value; return; }
                case 60: { v060 = value; return; }
                case 61: { v061 = value; return; }
                case 62: { v062 = value; return; }
                case 63: { v063 = value; return; }
                case 64: { v064 = value; return; }
                case 65: { v065 = value; return; }
                case 66: { v066 = value; return; }
                case 67: { v067 = value; return; }
                case 68: { v068 = value; return; }
                case 69: { v069 = value; return; }
                case 70: { v070 = value; return; }
                case 71: { v071 = value; return; }
                case 72: { v072 = value; return; }
                case 73: { v073 = value; return; }
                case 74: { v074 = value; return; }
                case 75: { v075 = value; return; }
                case 76: { v076 = value; return; }
                case 77: { v077 = value; return; }
                case 78: { v078 = value; return; }
                case 79: { v079 = value; return; }
                case 80: { v080 = value; return; }
                case 81: { v081 = value; return; }
                case 82: { v082 = value; return; }
                case 83: { v083 = value; return; }
                case 84: { v084 = value; return; }
                case 85: { v085 = value; return; }
                case 86: { v086 = value; return; }
                case 87: { v087 = value; return; }
                case 88: { v088 = value; return; }
                case 89: { v089 = value; return; }
                case 90: { v090 = value; return; }
                case 91: { v091 = value; return; }
                case 92: { v092 = value; return; }
                case 93: { v093 = value; return; }
                case 94: { v094 = value; return; }
                case 95: { v095 = value; return; }
                case 96: { v096 = value; return; }
                case 97: { v097 = value; return; }
                case 98: { v098 = value; return; }
                case 99: { v099 = value; return; }
                case 100: { v100 = value; return; }
                case 101: { v101 = value; return; }
                case 102: { v102 = value; return; }
                case 103: { v103 = value; return; }
                case 104: { v104 = value; return; }
                case 105: { v105 = value; return; }
                case 106: { v106 = value; return; }
                case 107: { v107 = value; return; }
                case 108: { v108 = value; return; }
                case 109: { v109 = value; return; }
                case 110: { v110 = value; return; }
                case 111: { v111 = value; return; }
                case 112: { v112 = value; return; }
                case 113: { v113 = value; return; }
                case 114: { v114 = value; return; }
                case 115: { v115 = value; return; }
                case 116: { v116 = value; return; }
                case 117: { v117 = value; return; }
                case 118: { v118 = value; return; }
                case 119: { v119 = value; return; }
                case 120: { v120 = value; return; }
                case 121: { v121 = value; return; }
                case 122: { v122 = value; return; }
                case 123: { v123 = value; return; }
                case 124: { v124 = value; return; }
                case 125: { v125 = value; return; }
                case 126: { v126 = value; return; }
                case 127: { v127 = value; return; }
                case 128: { v128 = value; return; }
                case 129: { v129 = value; return; }
                case 130: { v130 = value; return; }
                case 131: { v131 = value; return; }
                case 132: { v132 = value; return; }
                case 133: { v133 = value; return; }
                case 134: { v134 = value; return; }
                case 135: { v135 = value; return; }
                case 136: { v136 = value; return; }
                case 137: { v137 = value; return; }
                case 138: { v138 = value; return; }
                case 139: { v139 = value; return; }
                case 140: { v140 = value; return; }
                case 141: { v141 = value; return; }
                case 142: { v142 = value; return; }
                case 143: { v143 = value; return; }
                case 144: { v144 = value; return; }
                case 145: { v145 = value; return; }
                case 146: { v146 = value; return; }
                case 147: { v147 = value; return; }
                case 148: { v148 = value; return; }
                case 149: { v149 = value; return; }
                case 150: { v150 = value; return; }
                case 151: { v151 = value; return; }
                case 152: { v152 = value; return; }
                case 153: { v153 = value; return; }
                case 154: { v154 = value; return; }
                case 155: { v155 = value; return; }
                case 156: { v156 = value; return; }
                case 157: { v157 = value; return; }
                case 158: { v158 = value; return; }
                case 159: { v159 = value; return; }
                case 160: { v160 = value; return; }
                case 161: { v161 = value; return; }
                case 162: { v162 = value; return; }
                case 163: { v163 = value; return; }
                case 164: { v164 = value; return; }
                case 165: { v165 = value; return; }
                case 166: { v166 = value; return; }
                case 167: { v167 = value; return; }
                case 168: { v168 = value; return; }
                case 169: { v169 = value; return; }
                case 170: { v170 = value; return; }
                case 171: { v171 = value; return; }
                case 172: { v172 = value; return; }
                case 173: { v173 = value; return; }
                case 174: { v174 = value; return; }
                case 175: { v175 = value; return; }
                case 176: { v176 = value; return; }
                case 177: { v177 = value; return; }
                case 178: { v178 = value; return; }
                case 179: { v179 = value; return; }
                case 180: { v180 = value; return; }
                case 181: { v181 = value; return; }
                case 182: { v182 = value; return; }
                case 183: { v183 = value; return; }
                case 184: { v184 = value; return; }
                case 185: { v185 = value; return; }
                case 186: { v186 = value; return; }
                case 187: { v187 = value; return; }
                case 188: { v188 = value; return; }
                case 189: { v189 = value; return; }
                case 190: { v190 = value; return; }
                case 191: { v191 = value; return; }
                case 192: { v192 = value; return; }
                case 193: { v193 = value; return; }
                case 194: { v194 = value; return; }
                case 195: { v195 = value; return; }
                case 196: { v196 = value; return; }
                case 197: { v197 = value; return; }
                case 198: { v198 = value; return; }
                case 199: { v199 = value; return; }
                case 200: { v200 = value; return; }
                case 201: { v201 = value; return; }
                case 202: { v202 = value; return; }
                case 203: { v203 = value; return; }
                case 204: { v204 = value; return; }
                case 205: { v205 = value; return; }
                case 206: { v206 = value; return; }
                case 207: { v207 = value; return; }
                case 208: { v208 = value; return; }
                case 209: { v209 = value; return; }
                case 210: { v210 = value; return; }
                case 211: { v211 = value; return; }
                case 212: { v212 = value; return; }
                case 213: { v213 = value; return; }
                case 214: { v214 = value; return; }
                case 215: { v215 = value; return; }
                case 216: { v216 = value; return; }
                case 217: { v217 = value; return; }
                case 218: { v218 = value; return; }
                case 219: { v219 = value; return; }
                case 220: { v220 = value; return; }
                case 221: { v221 = value; return; }
                case 222: { v222 = value; return; }
                case 223: { v223 = value; return; }
                case 224: { v224 = value; return; }
                case 225: { v225 = value; return; }
                case 226: { v226 = value; return; }
                case 227: { v227 = value; return; }
                case 228: { v228 = value; return; }
                case 229: { v229 = value; return; }
                case 230: { v230 = value; return; }
                case 231: { v231 = value; return; }
                case 232: { v232 = value; return; }
                case 233: { v233 = value; return; }
                case 234: { v234 = value; return; }
                case 235: { v235 = value; return; }
                case 236: { v236 = value; return; }
                case 237: { v237 = value; return; }
                case 238: { v238 = value; return; }
                case 239: { v239 = value; return; }
                case 240: { v240 = value; return; }
                case 241: { v241 = value; return; }
                case 242: { v242 = value; return; }
                case 243: { v243 = value; return; }
                case 244: { v244 = value; return; }
                case 245: { v245 = value; return; }
                case 246: { v246 = value; return; }
                case 247: { v247 = value; return; }
                case 248: { v248 = value; return; }
                case 249: { v249 = value; return; }
                case 250: { v250 = value; return; }
                case 251: { v251 = value; return; }
                case 252: { v252 = value; return; }
                case 253: { v253 = value; return; }
                case 254: { v254 = value; return; }
                case 255: { v255 = value; return; }
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            for (int i = 0; i < 256; i++)
            {
                if (serelizateAnimationPoints[i] != -1 && serelizateAnimationPoints[i] < points.Count)
                {
                    animationPoints[i] = points[serelizateAnimationPoints[i]];
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            for (int i = 0; i < 256; i++)
            {
                if (animationPoints[i]!=null && points.Contains(animationPoints[i]))
                {
                    serelizateAnimationPoints[i] = points.IndexOf(animationPoints[i]);
                }
                else
                {
                    serelizateAnimationPoints[i] = -1;
                }
            }
            
        }
        
    }
}
