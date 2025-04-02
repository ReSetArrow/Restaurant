using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Models;

namespace Restaurant.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

//      (1) Microsoft.EntityFrameworkCore.SqlServer
//      (2) Microsoft.EntityFrameworkCore.Tools
//����T�{���\

//   Scaffold-DbContext "Data Source=C501A126;Database=Restaurant;TrustServerCertificate=True;User ID=abc;Password=123" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -NoOnConfiguring -UseDatabaseNames -NoPluralize -Force
//KIKIMAYCHAGISHY
//C501A126
//����T�{���\
//�ƥ�Models��Ƨ�

//�bRestaurantContext.cs�̼��g�s�u���Ʈw���{��
//      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//              => optionsBuilder.UseSqlServer("Data Source=���A����};Database=��Ʈw�W��;TrustServerCertificate=True;User ID=�b��;Password=�K�X");
//Program.cs�s�W
//builder.Services.AddDbContext<RestaurantContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantConnection")));
//appsettings.json�s�W
//"AllowedHosts": "*",
//  "ConnectionStrings": {
//    "RestaurantConnection": "Data Source=C501A126;Database=Restaurant;TrustServerCertificate=True;User ID=abc;Password=123"
//  }
//�s�WRoleController.cs
//�ƥ�Program��appsettings

//�s�WRole�����Ҿ�
//�b_Layout.cshtml�s�WBootstrap��CDN�s��
//    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
//    <link rel = "stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
//�}��Role/Create�T�{��Bootstrap���M��
//�ƥ�Shared

//�D�n�~�[
//<div class="container ">
//<div class="row justify-content-center">
//<div class="col-md-6 col-lg-4">
//</div>
//</div>
//</div>

//�s�Wconfirm�\�����e�X�\�঳�ĤG���T�{
//��class="d-block mx-auto"���Ϥ��m��
//�ק�Role/Create��ܪ��˺A�A�s�WGM
//�ק�Role/Delete��ܪ��˺A�A�R��GM
//�ק�Role/Details��ܪ��˺A
//�ק�Role/Edit��ܪ��˺A�A�ק�GM
//�ק�Role/Index��ܪ��˺A
//�ƥ�Role��Controllers�BView��wwwroot(Delete���Ϥ�)

//�ק�_Layout(���}�����L�k�T�w�b�s�����U��)

//�s�WPaymentsController
//�ק�Create�BDelete�BDetails�BEdit�BIndex��View
//���շs�W�\��Samsung Pay���פ����A�ץ���A���s�W����
//���շs�W�ɡA�D�䭫�Ʒ|�X�{�ҥ~�A�ݭn�ק�

