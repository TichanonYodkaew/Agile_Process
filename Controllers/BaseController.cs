using AgileRap_Process2.Data;
using Microsoft.AspNetCore.Mvc;

namespace AgileRap_Process2.Controllers
{
    public class BaseController : Controller
    {
        protected AgileRap_Process2Context db = new AgileRap_Process2Context(); //dbContext

    }
}
