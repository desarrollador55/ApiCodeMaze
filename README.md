# Paginación
La paginaciión es la obtención de ciertos resultados por medio del uso ciertos parametros de paginas sirve para poder lidiar de mejor manera cuando se tienen altas cantidades de datos (millones, miles de millón).

## PagedList
Esta clase que hereda de List y es genérica nos ayuda a establecer la lógica de skip/take, es decir, se establecen las páginas y sus elementos y permitirá seleccionar una página en específico. 

La clase se muestra a continuación:
```
public class PagedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }
    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
```

## QueryParameters
Para poder usar mejor la clase de PagedList debemos de establecer los parametros a usar del usuario, los cuales serían para establecer las páginas con las que se desea trabajar. La clase QueryParameters sería la clase base donde se establecen los elementos de páginas y ya se podrían crear más clases parameters para uso específico de distintos controllers.

Clase QueryParameters:
```
public abstract class QueryStringParameters
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
```

## Repository Logic
```
public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
        {
            var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                        o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);

            SearchByName(ref owners, ownerParameters.Name);

            return PagedList<Owner>.ToPagedList(owners.OrderBy(on => on.Name),
                ownerParameters.PageNumber,
                ownerParameters.PageSize);
}
```

En este fragmento de código vemos el uso de lógica para la construcción del equivalente a una "Consulta" donde se reciben owners con el método FindByCondition del repositorio genérico, al recibir el IQueryable buscamos por nombre específico y por último usamos el método de pagedList para transformarlo y devolver la lista paginada. Primero se sigue las restricciones de paginación propias (date y name) para después usar el ToPagedList para usar la paginación básica.

# Filtrado
El filtrado es un proceso que permite delimitar los resultados obtenidos por medio de una serie de definiciones de parametros, se diferencia de la busqueda dado que esta es más simple usando un único elemento (a menudo un string) para realizar la búsqueda, la filtración usa elementos distintos y que pueden ser mayores en cantidad para definir el resultado de la busqueda.

Implementación en OwnerParameters:

```
public class OwnerParameters : QueryStringParameters
{
    public uint MinYearOfBirth { get; set; }
    public uint MaxYearOfBirth { get; set; } = (uint)DateTime.Now.Year;
    public bool ValidYearRange => MaxYearOfBirth > MinYearOfBirth;
}
```

En el controlador se usa:
```
if (!ownerParameters.ValidYearRange)
    {
        return BadRequest("Max year of birth cannot be less than min year of birth");
    }
```

En OwnerRepository se añade el siguiente código:
```
var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth)
                            .OrderBy(on => on.Name);
```

# Busqueda
El método de busqueda es de los más básicos pero con altos niveles de usabilidad.

Implementación en OwnerParameters:
```

public class OwnerParameters : QueryStringParameters
{
    public uint MinYearOfBirth { get; set; }
    public uint MaxYearOfBirth { get; set; } = (uint)DateTime.Now.Year;
    public bool ValidYearRange => MaxYearOfBirth > MinYearOfBirth;
    public string Name { get; set; }
}
```

Implementación en OwnerRepository: 
```
public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
{
    var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);
    SearchByName(ref owners, ownerParameters.Name);
    return PagedList<Owner>.ToPagedList(owners.OrderBy(on => on.Name),
        ownerParameters.PageNumber,
        ownerParameters.PageSize);
}
private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
{
    if (!owners.Any() || string.IsNullOrWhiteSpace(ownerName))
        return;
    owners = owners.Where(o => o.Name.ToLower().Contains(ownerName.Trim().ToLower()));
}
```

# Sorteo
El sorteo se refiere al ordenamiento de datos, definido para que se ordene en una manera especificada.

Implementación en QueryStringParameters:
```
public abstract class QueryStringParameters
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
    public string OrderBy { get; set; }
}
```

Implementación en OwnerParameters:
```
public class OwnerParameters : QueryStringParameters
{
    public OwnerParameters()
    {
        OrderBy = "name";
    }
    public uint MinYearOfBirth { get; set; }
    public uint MaxYearOfBirth { get; set; } = (uint)DateTime.Now.Year;
    public bool ValidYearRange => MaxYearOfBirth > MinYearOfBirth;
    public string Name { get; set; }
}
```

Implementación en AccountParameters:
```
public class AccountParameters : QueryStringParameters
{
    public AccountParameters()
    {
        OrderBy = "DateCreated";
    }
}
```

Implementación en OwnerRepository (el siguiente código ocupa el paquete System.Linq.Dynamic.Core ):
```
private void ApplySort(ref IQueryable<Owner> owners, string orderByQueryString)
{
    if (!owners.Any())
        return;
    if (string.IsNullOrWhiteSpace(orderByQueryString))
    {
        owners = owners.OrderBy(x => x.Name);
        return;
    }
    var orderParams = orderByQueryString.Trim().Split(',');
    var propertyInfos = typeof(Owner).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    var orderQueryBuilder = new StringBuilder();
    foreach (var param in orderParams)
    {
        if (string.IsNullOrWhiteSpace(param))
            continue;
        var propertyFromQueryName = param.Split(" ")[0];
        var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
        if (objectProperty == null)
            continue;
        var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
        orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {sortingOrder}, ");
    }
    var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
    if (string.IsNullOrWhiteSpace(orderQuery))
    {
        owners = owners.OrderBy(x => x.Name);
        return;
    }
    owners = owners.OrderBy(orderQuery);
}
```

Implementación en OwnersRepository:
```
public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
{
    var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);
    SearchByName(ref owners, ownerParameters.Name);

    ApplySort(ref owners, ownerParameters.OrderBy);

    return PagedList<Owner>.ToPagedList(owners,
        ownerParameters.PageNumber,
        ownerParameters.PageSize);
}
```

# Versionamiento
Librerias a utilizar para el versionamiento:

- Microsoft.AspNetCore.Mvc.Versioning --5.10
- Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer --5.10

## Versionamiento por URL
Para poder hacer el versionamiento de URL, se debe de distinguir las clases de Controllers para diferenciar por versiones y ya en el mismo controlador se usará _ApiVersion("v")_ para establecer la versión. Una vez separados los controladores (puede ser por cambio de carpetas o dentro del mismo archivo pero con namespaces distintos para cada versión del controlador o en el caso de ruteo se puede poseer distintos métodos pero que son mapeados hacia una versión específica con la línea: _[MapToApiVersion("")]_) debemos de asegurarnos que los nombres de las rutas sean iguales para quese haga la asociación de ruteo, pero que vaya a ser divido por versión, como se muestra a continuación:

_[ApiVersion("1.0", Deprecated = false)]_
_Route("api/v{version:apiVersion}/[controller]")_

Como podemos observar en las líneas anteriores, especificamos por medio de ApiVersion la version a utilizar del controlador y junto a ella existe la opción de Deprecated para mostrar la información sobre si la versión en cuestión todavía posee soporte o no. Después de esto debemos de asegurarnos de inyectar los servicios en el program, a continuación se muestra un ejemplo:

```
builder_.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});


builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```

En AddApiVersioning se establecen ciertos parametros o reglas a seguir, como que se haga default a una version cuando no se específica cual, mostrar las versiones de Apis y un Reader para leer la versión a como se pida.

En AddVersionedApiExplorer primero se establece el formato que se desea utilizar (Nota: usar doble VV o VVVV no funciona). La segunda option, nos permite usar una version por medio de la URL a como esta indicadoen la ruta previamente mostrada como ejemplo.

## Versionamiento por Header, Query
Estos métodos no requieren de las modificaciones realizadas en el ruteo como el método de versionamiento anterior. Unicamente su declaración en el program. Aquí se usa un ApiVersionReader que permite juntar múltiples tipos en caso de que se llegue a desear usar el versionamiento por múltiples medios de manera concurrente.

```
builder_.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("x-version")
        );

});
```