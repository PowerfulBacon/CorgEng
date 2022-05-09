namespace GJ2022.DmiIconConversionUtility
{
    class IconState
    {

        public int spriteSheetPos = 0;
        //1, 4 or 8
        public int dirs = 1;
        public int frames;
        public float[] delay;
        public bool rewind;
        public bool loop;
        public bool movement;

    }
}
