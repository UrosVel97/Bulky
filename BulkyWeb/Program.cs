using Bulky.DataAccess;
using Bulky.DataAccess.Reporsitory;
using Bulky.DataAccess.Reporsitory.IReporsitory;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Bulky.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/*Ovaj servis je zaduzen da vrednosti propertija 'SecretKey' i 'PublishableKey' JSON objekta 'Stripe' u
 * fajlu 'appsettings.json' dodeli (injects) propertijima klase 'StripeSettings'. Nazivi propertija klase
 * 'StripeSettings' i nazivi propertija JSON objekta 'Stripe' moraju biti isti.
 */
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddRazorPages(); //Kada u nasim projektima imammo 'Razor Pages' onda je neophodno da definisemo servis koji omogucava da mi radimo i koristimo 'Razor Pages'

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();  /* Kada smo kreirali 'Scaffold Identity' automatski je kreiran
                                                                                                      * servis u 'Dependecy Injection Container-u' koji se zove 
                                                                                                      * 'AddDefaultIdentity()'. Ovaj servis kaze 'EntityFrameworkCore-u' da 
                                                                                                      * ce koristiti nasu bazu 'Bulky' pomocu klase 'ApplicationDbContext' da
                                                                                                      * kreira odgovarajuce tabele za korisnike i uloge kao i uloge korisnika.
                                                                                                      * Kada prvi put kreiramo 'Scaffold Identity' mi nemamo nikakve uloge za 
                                                                                                      * svakog novog korisnika koji se registruje. Da bi mogli da radimo sa ulogama
                                                                                                      * koje ce biti dodeljene korisnicima, onda moramo da koristimo 'AddIdentity<>()'
                                                                                                      * templejtsku metodu i da joj dodelimo dve klase 'IdentityUser' i 'IdentityRole'.
                                                                                                      * Na taj nacin mi smo naglasili da cemo svakom novokreiranom korisniku dodeliti
                                                                                                      * i ulogu. Moramo dodati i metodu 'AddDefaultTokenProviders()' da nam aplikacija
                                                                                                      * ne bio izbacivala gresku za email tokene
                                                                                                      * */

/*U slucaju ako kortisnik hoce da pristupi pogledu preko URL-a a da nema odgovarajuci pristup, tj. nije dodeljena
 * autorizacija za njegovu ulogu, onda ce se on preusmeriti na pogled koji prikazuje poruku 'Access Denied!'. Stim sto postoji
 * jedan problem a to je da aplikacija ima neku podrazumevanu putanju za taj pogled, ali se on ne nalazi na toj putanji.
 * Mi moramo u 'Program.cs' fajlu da naglasimo na kojoj putanji se nalazi taj pogled i eventualno neki drugi pogledi. 
 * Ovaj blok koda mora da se napise ispod servisa 'builder.Services.AddIdentity<>()', koji sluzi za rad sa korisnicima, ulogama,
 * ulogama korisnika itd. U suprotnom nece raditi.*/
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//Dodajemo servis koji nam omogucava da se registrujemo i loginujemo preko facebook-a
builder.Services.AddAuthentication().AddFacebook(option =>
{
    option.AppId = "1641620503231072";
    option.AppSecret = "e27d235e98e20cdb42b2205f9b291a5a";
});


/* Insancirali smo interfejs 'IDistributedCache' koji je zaduzen za to
 * da odredi gde ce se cuvati podaci sesije. ova metoda 'AddDistributedMemoryCache()'
 * kaze da ce se u memoriji servera cuvati kljuc i vrednost za odredjenu sesiju.
 * Mi mozemo da instanciramo interfejs 'IDistributedCache' tako da ima jedna zajednicka
 * memorija a da imamo vise instance nase aplikacije koje se nalaze na razlicitm serverima.
 */
builder.Services.AddDistributedMemoryCache(); 

//Registrovali smo servis koji je zaduzen za pravljenje 'Session' objekta.
//Sta ce sve od opcija biti ukljuceno za taj 'Session' objekat to smo naveli
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();/* Obavezno moramo da definisemo servis i njegov
                                                                        * 'Dependecy Injection Service Lifecyce', kada radimo sa 
                                                                        * promenljivama koje sluzi za pristup bazi i manipulaciju bazom.
                                                                        * Posto interfejs 'IUnitOfWork' cemo koristiti umesto objekta
                                                                        * klase 'ApplicationDbContext', onda moramo da definisemo servis u 
                                                                        * 'Dependecy Injection Container'.*/
builder.Services.AddScoped<IEmailSender, EmailSender>();//Ovde smo definisali servis koji ce slluziti za instanciranje promenljive interfejsa 'IEmailSender'

builder.Services.AddScoped<IDbInitializer, DbInitializer>(); //Registrujemo servis u DI kontejner koji ce da instancira promenljivu interfejsa 'IDbInitializer' sa objektom klase 'DbInitializer'
//Izmedju 'builder.Services.AddControllersWithViews()' i 
//'var app = builder.Build()' mi mozemo da dodajemo nove
//servise

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) //Ovde proveravamo da li je promenljivoj okruzenja "ASPNETCORE_ENVIRONMENT" dodeljena vrednost "Development".
                                      //Promenljivoj okruzenja "ASPNETCORE_ENVIRONMENT" mogu biti dodeljene i druge vrednosti kao i proizvoljne vrednosti
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Odavde mi konfigurisemo cevovod (pipeline) i dodajemo sve midleware-ove kroz
//koje ce proci neki http zahtev ka nasoj aplikaciji
app.UseHttpsRedirection();
app.UseStaticFiles();

//Kada radimo sa 'Stripe' moramo da podesimo 'API KEY' koji ustari cuva properti 'SecretKey' JSON objekta 'Stripe' u fajlu 'appsettings.json'
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.MapRazorPages();// I ova linija koda je obavezna kada radimo sa 'Razor Pages' u nasoj web aplikaciji. U suprotnom necemo moci da vidimo 'Razor Pages'

app.UseAuthentication(); /*Obavezno moramo da dodamo jos jedan middleware u pipeline. Ovaj middleware je zaduzen za autentikaciju
                          * korisnika koju pokusavaju da se login-uju, ako korisnik nije kreirao nalog onda nece ni moci da se login-uje.
                          * Takodje, 'UseAuthentication()' middleware mora da ide pre 'UseAuthorization()' middleware-a. Jer prvo se vrsi provera
                          * da li korisnik ima nalog, a posle se proverava koje sve uloge ima korisnik ako ima nalog.
                          */
                          
app.UseAuthorization();

app.UseSession(); //Obavezno u pipeline (cevovod) stavljamo midleware za rad sa sesijama

SeedDatabase(); //Kreiramo sve uloge i kreiramo korisnika koji je admin i pushujemo na bazu

//Ovde naglasavamo defaultnu (podrazumevanu) rutu, tj. koja stranica ce biti prikazana
//krajnjem korisniku kada hoce da pristupi nasoj web aplikaciji
app.MapControllerRoute(
    name: "default",
    /*Kada radimo sa 'Areas' u projektu, onda je neophodno da naglasimo u 'Program.cs' fajlu
     * koja je defaultna ruta kada krajnji korisnik hoce da otvori nasu web aplikaciju.
       Defaultna ruta mora da ima prvo naziv 'Area-e', zatim naziv kontrolera i na kraj naziv
       'Action' metode. Primer '/Area/Controller/Action/Id?' */
pattern: "{area=Kupac}/{controller=Home}/{action=Index}/{id?}");

app.Run();


//Ova metoda je zaduzena za seed-uje nasu bazu podataka, tako sto ce da kreira
//sve uloge za korisnike ako vec nisu kreirane i kreirace korisnika koji ce imati ulogu 'Admin'
void SeedDatabase()
{
    //Kreiramo objekat koji ce biti ustvari 'Scope servis'. Pomocu njega cemo pristupiti promenljivama koje taj servis instancira
    using(var scope=app.Services.CreateScope())
    {
        //Pomocu objekta koji je ustvari nas 'Scoped servis' vracamo promenljivu interfejsa 'IDbInitializer' koja je inicijalizovana sa objektom klase 'DbInitializer'
        var dbInitializer =scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        //Pozivamo metodu 'Initialize()' interfejsa 'IDbInitializer'. Ova metoda seeduje nasu bazu
        dbInitializer.Initialize();
    }
}