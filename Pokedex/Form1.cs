using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pokedex
{
    public partial class Form1 : Form
    {


        private static readonly HttpClient client = new HttpClient(); // Reutilizamos el cliente


        public Form1()
        {
            
            InitializeComponent();
            CargarPokemonAsync("pikachu");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }



        public class PokemonData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public SpriteData Sprites { get; set; }
            public List<TypeData> Types { get; set; }
        }

        public class TypeData
        {
            public TypeDetail Type { get; set; }
        }

        public class SpriteData
        {
            [JsonProperty("front_default")] // Indicar el nombre de la propiedad en el JSON
            public string FrontDefault { get; set; }
        }



        public class TypeDetail
        {
            public string Name { get; set; }
        }


        private async Task CargarPokemonAsync(string nombrePokemon)
        {
            string apiUrl = $"https://pokeapi.co/api/v2/pokemon/{nombrePokemon.ToLower()}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    PokemonData pokemon = JsonConvert.DeserializeObject<PokemonData>(jsonContent);

                    // Mostrar datos en consola
                    Console.WriteLine($"Nombre: {pokemon.Name}");
                    Console.WriteLine($"ID: {pokemon.Id}");
                    Console.WriteLine($"Sprite: {pokemon.Sprites.FrontDefault}");

                    Console.WriteLine("Tipos:");
                    foreach (var tipo in pokemon.Types)
                    {
                        Console.WriteLine($"- {tipo.Type.Name}");
                    }

                    // Asignar datos a los controles de la UI
                    lb2.Text = $"Nombre: {pokemon.Name}";
                    lblID.Text = $"ID: {pokemon.Id}";

                    lstTipos.Items.Clear(); // Limpiar la lista de tipos
                    foreach (var tipo in pokemon.Types)
                    {
                        lstTipos.Items.Add(tipo.Type.Name);
                    }

                    // Cargar la imagen de forma asíncrona
                    await CargarImagenAsync(pokemon.Sprites.FrontDefault);
                }
                else
                {
                    Console.WriteLine($"Error al cargar Pokémon. Código de estado: {response.StatusCode}");
                    MessageBox.Show($"Error al cargar Pokémon. Código de estado: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);
                MessageBox.Show("Error de red: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async Task CargarImagenAsync(string url)
        {
            try
            {
                using (var response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync();

                    if (stream.Length > 0) // Verificar que el stream no esté vacío
                    {
                        // Intentar cargar la imagen desde el stream
                        using (var image = Image.FromStream(stream))
                        {
                            pbSprite.Image?.Dispose(); // Liberar la imagen anterior si existe
                            pbSprite.Image = new Bitmap(image); // Asignar la nueva imagen
                        }
                    }
                    else
                    {
                        Console.WriteLine("El stream de la imagen está vacío.");
                        MessageBox.Show("No se pudo cargar la imagen del Pokémon.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar la imagen: " + ex.Message);
                MessageBox.Show("Error al cargar la imagen: " + ex.Message);
            }
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            string nombrePokemon = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(nombrePokemon))
            {
                await CargarPokemonAsync(nombrePokemon);
            }
            else
            {
                Console.WriteLine("Por favor, ingresa el nombre de un Pokémon.");
                MessageBox.Show("Por favor, ingresa el nombre de un Pokémon.");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void lb2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

