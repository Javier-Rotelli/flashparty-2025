using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.Samples.Cameras;
using TGC.MonoGame.Samples.Geometries;
using TGC.MonoGame.TP.Models;

namespace TGC.MonoGame.TP;

/// <summary>
///     Esta es la clase principal del juego.
///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
/// </summary>
public class TGCGame : Game
{
    public const string ContentFolder3D = "Models/kenney/";
    public const string ContentFolderEffects = "Effects/";
    public const string ContentFolderMusic = "Music/";
    public const string ContentFolderSounds = "Sounds/";
    public const string ContentFolderSpriteFonts = "SpriteFonts/";
    public const string ContentFolderTextures = "Textures/";

    private readonly GraphicsDeviceManager _graphics;

    private Effect _effect;
    private SimpleModel _cart;
    private Rails _rail_straight;
    private bool _isFreeCamera = false;

    private Camera _freeCamera;
    private Camera _fixedCamera;

    private Origin _origin;

    private Vector3 _cart_offset = new Vector3(0, 30, 110);

    /// <summary>
    ///     Constructor del juego.
    /// </summary>
    public TGCGame()
    {
        // Maneja la configuracion y la administracion del dispositivo grafico.
        _graphics = new GraphicsDeviceManager(this);

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

        // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
        // Carpeta raiz donde va a estar toda la Media.
        Content.RootDirectory = "Content";
        // Hace que el mouse sea visible.
        IsMouseVisible = true;
    }

    /// <summary>
    ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
    ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
    /// </summary>
    protected override void Initialize()
    {
        // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

        // Apago el backface culling.
        // Esto se hace por un problema en el diseno del modelo del logo de la materia.
        // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
        var rasterizerState = new RasterizerState();
        rasterizerState.CullMode = CullMode.None;
        GraphicsDevice.RasterizerState = rasterizerState;
        // Seria hasta aca.


        _freeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.UnitZ * 150);
        _fixedCamera = new StaticCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0f, 125f, -100f), new Vector3(0f, -0.05f, 1f), Vector3.Up);
        base.Initialize();
    }

    /// <summary>
    ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
    ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
    ///     que podemos pre calcular para nuestro juego.
    /// </summary>
    protected override void LoadContent()
    {
        _origin = new Origin(GraphicsDevice);
        // Cargo la textura del modelo.
        var colormap = Content.Load<Texture2D>(ContentFolder3D + "Textures/colormap");

        // Cargo un efecto basico propio declarado en el Content pipeline.
        // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
        _effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

        _cart = new SimpleModel(Content, ContentFolder3D + "coaster-train-front", _effect);
        _cart.Scale = new Vector3(1, 1, -1);
        _cart.Position = _cart_offset;

        _rail_straight = new Rails(Content, ContentFolder3D + "coaster-steel-straight", _effect);

        // Asigno la textura cargada al efecto.
        _effect.Parameters["Colormap"].SetValue(colormap);

        base.LoadContent();
    }


    private KeyboardState _previousKeyboardState;

    private bool OnKeyUp(Keys key)
    {
        return !Keyboard.GetState().IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
    }


    protected void UpdateModelPosition(GameTime gameTime)
    {
        if (OnKeyUp(Keys.W))
        {
            _cart_offset.Z += 10;
            System.Console.WriteLine("Offset: " + _cart_offset);
        }
        if (OnKeyUp(Keys.S))
        {
            _cart_offset.Z -= 10;
            System.Console.WriteLine("Offset: " + _cart_offset);
        }
        if (OnKeyUp(Keys.A))
        {
            _cart_offset.X -= 1;
            System.Console.WriteLine("Offset: " + _cart_offset);
        }
        if (OnKeyUp(Keys.D))
        {
            _cart_offset.X += 1;
            System.Console.WriteLine("Offset: " + _cart_offset);
        }
        if (OnKeyUp(Keys.R))
        {
            _cart_offset = new Vector3(0, 30, 110);
            System.Console.WriteLine("Offset: " + _cart_offset);
        }
        if (OnKeyUp(Keys.F))
        {
            _cart_offset.Y += 1;
            System.Console.WriteLine("Offset: " + _cart_offset);
        }
        if (OnKeyUp(Keys.V))
        {
            _cart_offset.Y -= 1;
            System.Console.WriteLine("Offset: " + _cart_offset);
        }
        _cart.Position = _cart_offset;
        _cart.Update(gameTime);
    }
    /// <summary>
    ///     Se llama en cada frame.
    ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
    ///     ante ellas.
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        // Aca deberiamos poner toda la logica de actualizacion del juego.
        KeyboardState state = Keyboard.GetState();
        // Capturar Input teclado
        if (state.IsKeyDown(Keys.Escape))
        {
            //Salgo del juego.
            Exit();
        }
        if (OnKeyUp(Keys.P))
        {
            _isFreeCamera = !_isFreeCamera;
        }

        //UpdateModelPosition(gameTime);
        _freeCamera.Update(gameTime);

        _rail_straight.Update(gameTime, 1000f);

        _cart.Update(gameTime);

        base.Update(gameTime);
        _previousKeyboardState = state;
    }

    /// <summary>
    ///     Se llama cada vez que hay que refrescar la pantalla.
    ///     Escribir aqui el codigo referido al renderizado.
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        // Aca deberiamos poner toda la logia de renderizado del juego.
        GraphicsDevice.Clear(Color.Black);
        var camera = _isFreeCamera ? _freeCamera : _fixedCamera;

        _origin.Draw(camera.View, camera.Projection);
        // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
        _effect.Parameters["View"].SetValue(camera.View);
        _effect.Parameters["Projection"].SetValue(camera.Projection);

        _cart.Draw();

        _rail_straight.Draw();
    }

    /// <summary>
    ///     Libero los recursos que se cargaron en el juego.
    /// </summary>
    protected override void UnloadContent()
    {
        // Libero los recursos.
        Content.Unload();

        base.UnloadContent();
    }
}