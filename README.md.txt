Sistema de Gestión de Almacén
1. Descripción del Proyecto
Este proyecto es una aplicación web de gestión de almacén desarrollada con ASP.NET Core MVC. Su objetivo es proporcionar una herramienta funcional para la administración de inventario, ventas y roles de usuario. La aplicación se basa en el patrón de diseño Modelo-Vista-Controlador (MVC), lo que garantiza una estructura clara, escalable y mantenible.
Características implementadas hasta ahora
Autenticación y Autorización: Gestión de roles de usuario (Admin, Dueño, Empleado, Proveedor) para restringir el acceso a funcionalidades específicas.
CRUD de Productos: Funcionalidad completa para crear, leer, actualizar y eliminar productos del inventario.
Ingreso de Productos: Permite registrar nuevos productos, tanto de forma manual como mediante el uso de un escáner de código de barras externo, que simula la entrada de un teclado.
Punto de Venta (Interfaz): Interfaz inicial para realizar ventas, donde se pueden agregar productos al carrito mediante escaneo de código de barras externo o entrada manual.
Comunicación Asíncrona (AJAX): Uso de llamadas AJAX para buscar productos en tiempo real en la interfaz de punto de venta, sin recargar la página.
2. Tecnologías y Herramientas
ASP.NET Core MVC: Framework para construir la aplicación web.
Entity Framework Core: ORM (Object-Relational Mapper) que utiliza el enfoque Code-First para interactuar con la base de datos.
ASP.NET Core Identity: Sistema de autenticación y autorización para la gestión de usuarios y roles.
SQL Server (localdb): Base de datos utilizada para almacenar la información.
Bootstrap 5: Framework de CSS para el diseño de la interfaz de usuario, garantizando un aspecto moderno y responsivo.
jQuery: Biblioteca de JavaScript utilizada para la manipulación del DOM y peticiones asíncronas (AJAX).
3. Configuración del Entorno y Ejecución
Para ejecutar el proyecto, necesitarás Visual Studio y el SDK de .NET instalado.
Clonar el repositorio:
bash
git clone https://github.com/tu-usuario/GestionAlmacen.git
Usa el código con precaución.

Abrir en Visual Studio: Abre el archivo de solución (GestionAlmacen.sln) en Visual Studio.
Restaurar paquetes NuGet: Visual Studio lo hará automáticamente, pero si es necesario, puedes ejecutar en la Consola del Administrador de Paquetes:
Update-Package
Crear y aplicar migraciones: La base de datos se creará automáticamente la primera vez que ejecutes el proyecto, gracias a los comandos de migración de Entity Framework. Asegúrate de que la cadena de conexión en appsettings.json esté correctamente configurada.
Ejecutar el proyecto: Presiona F5 en Visual Studio para iniciar la aplicación. Los roles y el usuario administrador inicial se crearán automáticamente.
4. Estructura y Lógica del Código
4.1. Configuración Inicial (Program.cs)
En el archivo Program.cs, se configura la aplicación y se inicializan los roles y un usuario administrador inicial.
Lógica de inicialización de roles: Al arrancar la aplicación, se asegura que los roles de Admin, Dueño, Empleado y Proveedor existan en la base de datos.
Creación de usuario administrador: Si no existe, se crea un usuario con la cuenta admin@almacen.com y se le asigna el rol de Admin, facilitando el inicio del sistema.
4.2. Modelo de Datos (Models y Data)
Clases de Entidad (Models):
Producto.cs: Define las propiedades del producto (como Nombre, Precio, Stock y CodigoBarra). La anotación [Required] asegura la validación de datos.
Cliente.cs: Clase que representa a los clientes.
Venta.cs: Almacena la información principal de una transacción de venta.
VentaDetalle.cs: Almacena los detalles de cada producto vendido en una transacción.
Contexto de Base de Datos (Data/ApplicationDbContext.cs): Hereda de IdentityDbContext e incluye los DbSet para los modelos personalizados, actuando como puente entre el código y la base de datos.
ViewModels/VentaViewModel.cs: Clase temporal usada para manejar el estado del carrito de compras en la memoria, antes de que la venta sea finalizada.
4.3. Controladores y Vistas (ProductosController y VentasController)
4.3.1. ProductosController
Autorización: El atributo [Authorize(Roles = "Admin")] restringe el acceso a las funcionalidades de gestión de productos solo a los usuarios con el rol de Admin.
Manejo de Formularios: El método Create ([HttpPost]) recibe los datos del formulario. El valor del CodigoBarra, ya sea ingresado manualmente o mediante un escáner externo, es enlazado al modelo Producto.
Validación de CodigoBarra (Opcional): Se incluye una verificación en el lado del servidor para asegurar que no se creen productos con códigos de barras duplicados.
Entrada de Escáner Externo: La vista Create.cshtml utiliza el atributo autofocus en el campo CodigoBarra para que el escáner (que actúa como un teclado) ingrese el código directamente. Un script de jQuery evita que el formulario se envíe al presionar Enter.
4.3.2. VentasController
Búsqueda de Productos (AJAX): La acción BuscarProducto ([HttpPost]) recibe un código de barras del cliente, busca el producto en la base de datos y devuelve el resultado en formato JSON. Esta petición se realiza de forma asíncrona.
Interfaz de Punto de Venta (Views/Ventas/PuntoVenta.cshtml):
Un campo de texto con autofocus recibe la entrada del escáner externo o manual.
Un script de JavaScript utiliza jQuery para:
Detectar la pulsación de Enter.
Realizar una llamada AJAX a Ventas/BuscarProducto.
Actualizar dinámicamente el listado del carrito de compras y el total de la venta en la interfaz.
5. Próximos Pasos
Completar la lógica de ventas: Implementar la acción para finalizar la venta, guardar la transacción en la base de datos y descontar el inventario de forma segura.
Manejar productos sin código de barras: Añadir una funcionalidad de búsqueda por nombre en la interfaz de punto de venta.
Implementar el módulo de fiados: Desarrollar la gestión de clientes, registrar ventas a crédito y gestionar los saldos pendientes.
Mejorar la interfaz de usuario: Añadir mejoras visuales y de usabilidad en las distintas vistas.
Sistema de Reportes: Crear reportes básicos de ventas y stock para el usuario Dueño.
