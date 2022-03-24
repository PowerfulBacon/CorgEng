using CorgEng.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering.Shaders
{
    public class ShaderSet
    {

        //Relative directory of the shaders to the .exe
        public const string SHADER_DIRECTORY = "./Content/Shaders/";

        private static Dictionary<string, ShaderSet> loadedShaders = new Dictionary<string, ShaderSet>();

        uint vertex_shader;
        uint fragment_shader;

        public ShaderSet(string name, string base_directory = SHADER_DIRECTORY)
        {
            //Check for cached shaders
            if (loadedShaders.ContainsKey(name))
            {
                vertex_shader = loadedShaders[name].vertex_shader;
                fragment_shader = loadedShaders[name].fragment_shader;
                return;
            }
            //Generate the shaders
            vertex_shader = glCreateShader(GL_VERTEX_SHADER);
            fragment_shader = glCreateShader(GL_FRAGMENT_SHADER);
            //Load and compile shaders
            try
            {
                //Read the shader sources
                glShaderSource(vertex_shader, File.ReadAllText($"{base_directory}{name}/{name}.vert"));
                glShaderSource(fragment_shader, File.ReadAllText($"{base_directory}{name}/{name}.frag"));
                //Compile the shaders
                glCompileShader(vertex_shader);
                glCompileShader(fragment_shader);
                //Print the result of compiling the shader
                string vertInfoLog = glGetShaderInfoLog(vertex_shader);
                string fragInfoLog = glGetShaderInfoLog(fragment_shader);
                if (vertInfoLog != string.Empty || fragInfoLog != string.Empty)
                {
                    Log.WriteLine($"{name}.vert: {vertInfoLog}");
                    Log.WriteLine($"{name}.frag: {fragInfoLog}");
                    Log.WriteLine("Shaders failed to compile.");
                }
                else
                {
                    //Print a debug message
                    Log.WriteLine($"Loaded {name} shaders successfully.");
                }
                //Cache the shaders for faster loading later
                loadedShaders.Add(name, this);
            }
            catch (Exception e)
            {
                //Error in the file.
                Log.WriteLine($"ERROR: Failed to compile vertex shaders. {name}.vert and {name}.frag!", LogType.ERROR);
                Log.WriteLine(e.Message, LogType.ERROR);
                //Delete shaders.
                DeleteShaders();
                return;
            }
        }

        public void AttachShaders(uint program_uint)
        {
            //Attaches the shaders to the program.
            glAttachShader(program_uint, vertex_shader);
            glAttachShader(program_uint, fragment_shader);
        }

        public void DetatchShaders(uint programUint)
        {
            glDetachShader(programUint, vertex_shader);
            glDetachShader(programUint, fragment_shader);
        }

        public void DeleteShaders()
        {
            //Deletes the shaders for shutdown.
            glDeleteShader(vertex_shader);
            glDeleteShader(fragment_shader);
        }

        public uint GetVertexShader()
        {
            return vertex_shader;
        }

        public uint GetFragmentShader()
        {
            return fragment_shader;
        }

    }
}
