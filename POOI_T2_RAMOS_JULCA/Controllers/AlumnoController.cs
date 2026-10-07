using Newtonsoft.Json;
using POOI_T2_RAMOS_JULCA.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace POOI_T2_RAMOS_JULCA.Controllers
{
    public class AlumnoController : Controller
    {
        // Ruta del archivo JSON
        private string RutaArchivo
        {
            get
            {
                return Server.MapPath("~/App_Data/alumnos.json");
            }
        }

        // Deserializar: JSON -> List<Alumno>
        private List<Alumno> LeerAlumnos()
        {
            if (!System.IO.File.Exists(RutaArchivo))
            {
                return new List<Alumno>();
            }

            string json = System.IO.File.ReadAllText(RutaArchivo);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Alumno>();
            }

            return JsonConvert.DeserializeObject<List<Alumno>>(json)
                   ?? new List<Alumno>();
        }

        // Serializar: List<Alumno> -> JSON
        private void GuardarAlumnos(List<Alumno> alumnos)
        {
            string json = JsonConvert.SerializeObject(
                alumnos,
                Formatting.Indented
            );

            System.IO.File.WriteAllText(RutaArchivo, json);
        }

        // GET: Alumno/Index
        public ActionResult Index()
        {
            List<Alumno> alumnos = LeerAlumnos();

            return View(alumnos);
        }

        // GET: Alumno/Agregar
        [HttpGet]
        public ActionResult Agregar()
        {
            return View(new Alumno());
        }

        // POST: Alumno/Agregar
        [HttpPost]
        public ActionResult Agregar(Alumno alumno)
        {
            List<Alumno> alumnos = LeerAlumnos();

            // Validar que el DNI no se repita
            bool existe = alumnos.Any(a => a.dni == alumno.dni);

            if (existe)
            {
                ViewBag.Mensaje = "El DNI ingresado ya se encuentra registrado.";

                return View(alumno);
            }

            alumnos.Add(alumno);

            GuardarAlumnos(alumnos);

            TempData["Mensaje"] = "Alumno registrado correctamente.";

            return RedirectToAction("Index");
        }

        // GET: Alumno/Eliminar
        public ActionResult Eliminar(string dni)
        {
            List<Alumno> alumnos = LeerAlumnos();

            Alumno alumno = alumnos.FirstOrDefault(
                a => a.dni == dni
            );

            if (alumno != null)
            {
                alumnos.Remove(alumno);

                GuardarAlumnos(alumnos);

                TempData["Mensaje"] = "Alumno eliminado correctamente.";
            }

            return RedirectToAction("Index");
        }
        // GET: Alumno/Detalles
        public ActionResult Detalles(string dni)
        {
            List<Alumno> alumnos = LeerAlumnos();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno == null)
            {
                return HttpNotFound();
            }

            return View(alumno);
        }


        // GET: Alumno/Actualizar
        [HttpGet]
        public ActionResult Actualizar(string dni)
        {
            List<Alumno> alumnos = LeerAlumnos();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno == null)
            {
                return HttpNotFound();
            }

            return View(alumno);
        }


        // POST: Alumno/Actualizar
        [HttpPost]
        public ActionResult Actualizar(string dniOriginal, Alumno alumno)
        {
            List<Alumno> alumnos = LeerAlumnos();

            Alumno alumnoExistente =
                alumnos.FirstOrDefault(a => a.dni == dniOriginal);

            if (alumnoExistente == null)
            {
                return HttpNotFound();
            }

            // Verificar que el nuevo DNI no pertenezca a otro alumno
            bool dniRepetido = alumnos.Any(
                a => a.dni == alumno.dni &&
                     a.dni != dniOriginal
            );

            if (dniRepetido)
            {
                ViewBag.Mensaje =
                    "El DNI ingresado ya pertenece a otro alumno.";

                return View(alumno);
            }

            alumnoExistente.dni = alumno.dni;
            alumnoExistente.nombres = alumno.nombres;
            alumnoExistente.apellidos = alumno.apellidos;
            alumnoExistente.carrera = alumno.carrera;
            alumnoExistente.ciclo = alumno.ciclo;

            GuardarAlumnos(alumnos);

            TempData["Mensaje"] =
                "Alumno actualizado correctamente.";

            return RedirectToAction("Index");
        }

    }
}