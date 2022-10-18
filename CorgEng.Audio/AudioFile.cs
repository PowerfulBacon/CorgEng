using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using Silk.NET.OpenAL;
using System;
using System.Buffers.Binary;
using System.IO;

namespace GJ2022.Audio
{
    internal unsafe class AudioFile
    {

        [UsingDependency]
        private static ILogger Logger = null!;

        public short ChannelCount { get; } = -1;
        public int SampleRate { get; } = -1;
        public int ByteRate { get; } = -1;
        public short BlockAlign { get; } = -1;
        public short BitsPerSample { get; } = -1;
        public BufferFormat BufferFormat { get; } = 0;
        public float PlayTime { get; } = 0;

        public uint Buffer { get; }

        public AudioFile(string filePath)
        {
            ReadOnlySpan<byte> fileData = File.ReadAllBytes(filePath);
            //Check for file format
            if (fileData[0] != 'R' || fileData[1] != 'I' || fileData[2] != 'F' || fileData[3] != 'F')
                throw new FileLoadException($"File {filePath} is not in RIFF format and cannot be loaded");
            if (fileData[8] != 'W' || fileData[9] != 'A' || fileData[10] != 'V' || fileData[11] != 'E')
                throw new FileLoadException($"File {filePath} is not in WAVE format and cannot be loaded");
            //Generate a buffer
            AL alApi = AL.GetApi();
            Buffer = alApi.GenBuffer();
            //Load data
            int index = 12;
            //Read the audio data
            while (index + 4 < fileData.Length)
            {
                string identifier = $"{(char)fileData[index++]}{(char)fileData[index++]}{(char)fileData[index++]}{(char)fileData[index++]}";
                int blockSize = BinaryPrimitives.ReadInt32LittleEndian(fileData.Slice(index, 4));
                //Move forward 4 bytes
                index += 4;
                //Check the identifiers
                switch (identifier)
                {
                    case "fmt ":
                        //Check for the block size
                        if (blockSize != 16)
                            throw new Exception($"Unknown audio format with chunk size {blockSize} in file {filePath}");
                        //Read the audio format
                        int audioFormat = BinaryPrimitives.ReadInt16LittleEndian(fileData.Slice(index, 2));
                        index += 2;
                        if (audioFormat != 1)
                            throw new Exception($"Unknown audio format ({audioFormat}) in file {filePath}");
                        //Read the fmt data
                        ChannelCount = BinaryPrimitives.ReadInt16LittleEndian(fileData.Slice(index, 2));
                        index += 2;
                        SampleRate = BinaryPrimitives.ReadInt32LittleEndian(fileData.Slice(index, 4));
                        index += 4;
                        ByteRate = BinaryPrimitives.ReadInt32LittleEndian(fileData.Slice(index, 4));
                        index += 4;
                        BlockAlign = BinaryPrimitives.ReadInt16LittleEndian(fileData.Slice(index, 2));
                        index += 2;
                        BitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(fileData.Slice(index, 2));
                        index += 2;
                        //Parse buffer format
                        BufferFormat = GetBufferFormat();
                        break;
                    case "data":
                        ReadOnlySpan<byte> data = fileData.Slice(44, blockSize);
                        //Increment the index
                        index += blockSize;
                        //Fill the buffer data
                        fixed (byte* dataPointer = data)
                            alApi.BufferData(Buffer, BufferFormat, dataPointer, blockSize, SampleRate);
                        break;
                    case "JUNK":
                        index += blockSize;
                        break;
                    case "iXML":
                        //Do nothing
                        index += blockSize;
                        break;
                    default:
                        index += blockSize;
                        Logger.WriteLine($"Unknown section of data read from {filePath} ({identifier})", LogType.ERROR);
                        break;
                }
            }
            //Calculate playtime
            alApi.GetBufferProperty(Buffer, GetBufferInteger.Size, out int bufferSize);
            alApi.GetBufferProperty(Buffer, GetBufferInteger.Frequency, out int frequency);
            PlayTime = (float)bufferSize / (frequency * ChannelCount * BitsPerSample / 8);
            Logger.WriteLine($"Loaded sound {filePath} successfully! ({ChannelCount} channels, {SampleRate} sampling rate, {ByteRate} byte rate, {BlockAlign} block align, {BitsPerSample} bits per sample, length: {PlayTime}s)", LogType.DEBUG);
        }

        private BufferFormat GetBufferFormat()
        {
            if (ChannelCount == 1)
            {
                if (BitsPerSample == 8)
                    return BufferFormat.Mono8;
                else if (BitsPerSample == 16)
                    return BufferFormat.Mono16;
                Logger.WriteLine($"Cannot play mono sound with {BitsPerSample} bits per sample.", LogType.ERROR);
            }
            else if (ChannelCount == 2)
            {
                if (BitsPerSample == 8)
                    return BufferFormat.Stereo8;
                else if (BitsPerSample == 16)
                    return BufferFormat.Stereo16;
                Logger.WriteLine($"Cannot play stereo sound with {BitsPerSample} bits per sample.", LogType.ERROR);
            }
            else
                Logger.WriteLine($"Cannot play [unknown] sound with {BitsPerSample} bits per sample.", LogType.ERROR);
            throw new ArgumentException("Unable to identify buffer format.");
        }

    }
}
