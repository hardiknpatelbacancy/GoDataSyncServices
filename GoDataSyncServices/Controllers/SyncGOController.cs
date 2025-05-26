using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Net.Http.Headers;
using System.Text.Json;
using GoDataSyncServices.RequestModels;

namespace GoDataSyncServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncGOController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private const string ApiBaseUrl = "https://dev-portal-api.include.com";
        private const string AuthToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IlY1RGxhZmhBVVZDLUpwWG1ST2p0NCJ9.eyIvcm9sZXMiOlsiMDM0MmI5NTAtMDY0ZC01ZTI5LWIyY2QtYzQ0MGI1ZjUyMjA2IiwiMDU2M2MyMDctMzFhMi01ZDUyLTljNjgtY2RjMjBhNTViMjZlIiwiMDg4OWJhN2YtNDAxNC01ZTI2LWI4NTgtZGE5MDRmNzU0Mjc3IiwiMDhkM2VjMTUtMzZhMy01NzY2LTgwMjUtZmMwNGI5ODU4MGI1IiwiMDk1MTVlYTItZDAyMy01NDBlLWE1NGMtZDMxMzQwYjYzYjIzIiwiMDlhMzJkZjMtOTQ4ZS01NzM5LWI2ZjgtYThmMGZhNzY1YjdhIiwiMGE3ODNiOWEtY2YwZC01MjYyLThiNzUtNmM4YWZjMmM0YzE1IiwiMGJkNzBjMmUtYzA4Zi01NDU5LTk5ZjEtZTEzODMxN2FiNDNiIiwiMGM4ZGFhOTEtOTdkMS01OThlLWE3MzgtYWJiNzQ0MmZjYTMzIiwiMGNhYjJmM2UtMjA1ZC01NTVjLWEzZmQtOWY2YTY0ZmYwNGY3IiwiMGU0MTExNTItY2M3MC01Y2E3LTg0ODktMmM1NmYzNDY4NTNkIiwiMTEwNjk5ZDctNzBlZi01NjRjLWFjYzEtYmRiMGJmZDE1MmUzIiwiMTE3Zjg5YzYtYjY0NS01M2M0LTk3ODctOWM3OTE0MmJiMDZmIiwiMTI2NDAwNzQtYWFjOS01ZjNmLTk1MTgtY2YzMzE4NTIwMTA0IiwiMTJjMmJlYTYtZGM0ZS01OTViLWEyZWItYjZiNTQwNjA5YTk3IiwiMTNkN2E4YzMtZDM4Ny01ZmRlLTgzOTMtZTQyMTE5ZDVlOGU3IiwiMTUxM2I0OWQtYWYzYi01MWQ3LWFlMTAtZGNiOWYwNzZhNTFmIiwiMTU0ZTNmYTktZWQzMi01ZGRmLWEyZTQtMjBjMmJlNTk1NWEzIiwiMTY0ZTU2MjktZTEzYi01YmQyLTk3NjMtOWQyYjUzNTczZDZjIiwiMTc0ZGNhMzMtZGJhZi01MGYyLTgxNGYtZjFhN2RjNDE3YWU0IiwiMTdjYzA0NzMtMDhkMi01YmFmLThiNDUtNjJkMTUwNWIyNjRmIiwiMTgwOTRkNDktOGFmYi01ZDIwLTg4ODEtZjJjYzE5NWQyMTQ0IiwiMTgzZTE4ZjMtOWQ1My01MzBjLWE5ODctY2YyZTRlYzUzYjRkIiwiMTk1YmM2ZWEtYThjNi01NjNjLWFlNDMtNTM4ZGU2ZTE4NzE0IiwiMWFiMjQ3OWQtZjkwMC01YzgxLTkyNjMtZjcyYTA4ZTdhMWUxIiwiMWMwMWI3ZWMtZmRkYS01OTVjLTkyMDgtNDM3NDY5MzgwNmEwIiwiMWM1Y2RjZGMtN2VkNC01MzM1LWE0MDctNDIyODYyMjk4NmY3IiwiMWNhZmMyNTEtOWEwOC01YjRlLWE0NDEtN2E0ZjQ3OGYxYTc2IiwiMWViMmJkMjQtNGU1Ny01MjgzLWE4ZmUtNmYyODRiZjc5ZTU2IiwiMjMwZGVjYzUtZjUxYS01MDg5LWEyNjQtOWQ5NWQ5OWEwZTRkIiwiMjQzODYwYjktYTI3Yi01YTI5LTk4YjItZjcyMTliNGY5NTJkIiwiMjRkZmE2M2UtNTY5ZS01OTMzLWFlNzctMmYwMGY2OTU3NzEzIiwiMjU1MGY0NzctY2RlMS01MDY0LWIyNDUtMWQ0MjZlZGI2MGE0IiwiMjhlNGMzMzUtOWNjMy01NjdlLWE1ZWItNTMzMThkNDMxNzgyIiwiMjk5YWZmZDMtMGRhMS01M2RkLWFiZTYtODRjZjY1ZmUzNDA2IiwiMjlkMzVlMjUtZjZlZS01N2JhLWI1OWEtNTI2ZWZjYzIxZjUyIiwiMmFjNzAwOTctNjI0ZC01ZWI4LTg2YzItNjYwMjBiNTQ3NzdlIiwiMmMzODgzM2MtOGViZS01ZGFmLTkzNTMtZDUzNjUxMjEyYjIxIiwiMmNkN2I2M2YtNTQzNS01ZTViLTg2MjgtNGYyODcyYzcyZjFmIiwiMmNlMmU2NDgtYTE4OS01MDFlLWFkYTctNzU2OGQzMzg2NDA3IiwiMmYyMWNlMmQtYzY4Yy01ZmZmLWEyNTItMmM5MmM4MDliZWY0IiwiMmY4YTQ2ZjMtMTg5ZC01YTg2LThkODQtNTA4ZjM1YTVhMjdmIiwiMzA3ZGI1OWUtYjY2My01MDUxLTg2NzktM2EyOTQ5YWEyMzA1IiwiMzEwODkyZmEtMjUwNi01YjMwLWJiMmQtNjdiMTY5YjA5ZDBlIiwiMzFiMjEzMDEtYjc4Yy01MWMzLTg1N2ItMDNkMWMxOWY2NDA2IiwiMzFkZTQwNjEtM2JlMS01ZDZiLThiNGItZTEyZDRmMjM4MTUyIiwiMzM0NjljOGUtNzI4ZS01NmJkLWE5YzItMzQ3MGMwZTE1YjAyIiwiMzM3M2VjNGQtYTgwZC01NDE5LTlhOTEtODVhNzhmNDU0M2Q0IiwiMzY3MWUzYjMtOWMzZC01NjQ5LTk3NjEtMDdlNzMwYjExZGZiIiwiMzZlMWUyNDgtYjE3NC01OWQzLTg1OTEtMjBlYjU2NjlkMTcyIiwiMzczZTVkZTItNGI5My01NzI0LTliYWEtZGJhNjMwNWU0NWRmIiwiMzk3OTQ1MGMtYzA3OC01MzhjLWE2ZDgtZGMwYzdhYjAzMTJjIiwiMzlhYzMwOTMtMWJiMS01YzdmLWJmYmItYjFjZDJiZDViZmExIiwiMzlkYWFkZTUtMGZlNS01MTJiLWJmYmMtZWQzZjkzNDY3YmFmIiwiM2FmY2Y1MjgtYjg4MS01YzgwLWI3NTctMmI3YjIyNjZhOWNiIiwiM2MzMDNkNjEtMWIyNS01ZWIyLTlhZjAtZjViMmM1YWU3MDkwIiwiM2Q3YzE3NDktNzk5ZS01MWZmLWFiNzAtODcwZTI3OGVmNjMyIiwiM2U2MTQ2OWEtMmJkMi01NjZmLWFhNmEtOTgyY2I0MjVlZmEwIiwiNDA2MTUzOGItNDBjYS01NDM0LWI4MDgtMTUzNTYwZjE1ODRmIiwiNDU2OWM5ZDEtMTRmOS01OWI2LTllZTMtZTgwMTk1MzBhNjY5IiwiNDZhMzkyNWItOTkyNy01MzY3LTkxYWUtNDI5ZmMxMTljNDlmIiwiNDgxZjZiYWEtYmEwOC01OTdmLTlkNTgtMjE3YWI4MjUzMWM3IiwiNDliMTBmYWQtNTI5YS01ZTdlLWFlZDQtODVlMWZkNmVkZTBhIiwiNGMxMGRlNTItZGQyMi01ZGY5LWJiZjctYjhjNjUwYWU2ZGYwIiwiNGNjNGZkZjEtZTdlNS01MzgyLWE4NzYtYzcxZDEzNzMwMGJlIiwiNGRhODI5ZDUtOTAyMy01MmYwLTk3NDUtM2RkMzc2ZDJkMDYxIiwiNGVmZjlmNjEtOTgyOS01OWIwLThjMmUtYThiMTFhMTk0N2YxIiwiNGZjODJhYzYtMmZmYy01M2JjLWEwNzYtZjQ1ZDg4YTc3N2ZkIiwiNTBiNWQ5YzAtZTNhNi01YWIzLTkzN2ItOTMwMGY1MTRjYWYyIiwiNTEzNDY2YTctYjM3ZC01ZTBlLTg5NTgtODg5YTI1NmQzYjc3IiwiNTE2NmJmZGYtYzA3MC01NjQ3LWIyNWYtMGI3M2Y1NmU0ZjFmIiwiNTFhMTUyNjctY2MxOS01YjMwLTllYzEtZTBhMDRhNWVlOTYwIiwiNTNkOWEyZTQtNTZmMS01MmMyLTk1ZGQtZjIwMzVlMTYyNGQwIiwiNTU1N2QzNDQtZjhkOS01NzZkLWI2ZDgtMzI1MzE3YzNkY2IyIiwiNTZmMmFiOTQtY2QwMS01MDE4LWJiMmQtYzUxMGQ2Yjg3NTEwIiwiNTc0YjIzNzAtNWZjYi01YTkyLWExYzUtMzliZmVjMjI0Y2ZlIiwiNTc3MjhjZTMtMTQ2Ny01MGZmLTllMDItOWE1MWM0YzZiZGYxIiwiNTdiYTcxYTMtZmEzNi01NTA1LWE2ZGEtYjIyMmFiMDZiOThmIiwiNTdkMDliMmEtNjc5MS01YTRiLWI4YmQtZWFhMWVjZDliZTFiIiwiNTk2NWY3ZTYtNzZhMS01Y2RlLWI3MDEtMjk4N2ViOTZhMjIzIiwiNWE4NjkzYzUtNjY3NS01MjI1LThkY2YtODIzZDg3NTBmOTBiIiwiNWFkNWVkZDAtZTAyNy01NzdiLWJlZDAtYjgwMTMzN2RkOGFiIiwiNWI2Njc3NGEtMmU1OC01NmJjLWJjZmEtZjJiYTIwNmU4ZGI4IiwiNWQ3ZTM4OTctNDI3MS01NzExLWE0NzctYzE3YjNkNTE3ZDU1IiwiNWRiOWYyMDItMmNkNC01MmE0LThhN2QtMTQ5NDgxOGU1NTk4IiwiNWRmOWY3ZWQtZDMwMy01ZDJmLWJmMWYtMzhmOTBlMjRiMjE1IiwiNjBhZjkyZWUtYWRmYy01Nzg2LWFiZmEtNzU5YWI5MjlhMWY0IiwiNjE3NzdkNGMtZTI1Yi01ZTMxLWIzNTAtOGQ4MjVkMDk3ZGIzIiwiNjJkNzU3NTAtMjEyZi01N2Q1LTkwM2YtOTQ2NTM5NmM4OTYwIiwiNjU4OTBhOWYtNjJlZC01N2I4LWE4NmQtOWU4MzY3MjJhODMyIiwiNjVlZGQxOWMtYWNkMS01ODljLTlmMWEtYzE1MzliMGQzODhmIiwiNjYwMWU4MDctOTMwYi01ZTI1LWI1OTktYzdiYjllZWZkZjc4IiwiNjg4Y2YzZWUtZjYyYS01OTI2LWEyZDItYTM0MzJiODcxYzk1IiwiNjkyYjI0YWQtMGE4Zi01NWE3LThkZGEtOWNhMzM5MjNiNDI1IiwiNjk4ZTQwZjgtMjdkYS01Mjc2LTg4ODQtNDBhYzFjYmMwOTllIiwiNmEzOTQ3OWItYWE3Yi01OTEzLWIxOWQtY2I2YTdjYjJiOWQ2IiwiNmE0ODJiMTAtM2MyYS01NzI1LTlmY2MtZDNiNzg4YTFlOTEyIiwiNmM3NTg0NWMtMGYyNy01ZjAzLTk4YjEtOWZkZmRhZTQ4OTBkIiwiNmQ1MDczMDctNmJlZC01NDVlLTk3ODktYzNjNGUzYmNkZWE5IiwiNmY2Mzg3MzMtNTJiZi01MjhlLWE3YWItMjcxOWRjOWFjYzYyIiwiNzA5YzE0ZmYtZDlkYi01ZGI3LWFhN2EtYmRiNGE4MGI4NzVkIiwiNzE0NTI2OTUtYTI1Yi01ODc2LWI2NWUtZGNhODk0NzM1YmRhIiwiNzE2YjQyMTUtYzZmYy01NjIwLTk5M2MtNDQ1Y2M3YjVkZjZiIiwiNzE4M2NjMTUtMjhhYi01ZWU1LThlOTctMTE5MjMyYWViYjZhIiwiNzE5MGJjOGMtYjQxYy01MmNiLWI0NTgtYjg5ZGFhNDk3ODdjIiwiNzE5ODBlZTMtOTI4NC01NmQ3LTlkNDktZDZhOTU5ODU0ODk5IiwiNzI5ZWM0NWQtOTZlNi01NmVkLWFjODUtZjY0OTM4ZmVlMTM2IiwiNzdiYjc0MmMtNGQ2ZC01ZWQ3LThlYmItNmY2YWVkZGIzYzc3IiwiNzg4NmIwYzItY2JlZS01M2ZjLWE1MmMtNTRkYzU0MzcyMjkxIiwiNzljODk3ZGMtYWFkNi01Yjg4LTljOTctYzE4MTEwMDRlNzQyIiwiN2I2OGY2NGEtYjJiNC01YjUwLTk0MzQtYzA1MTY2OGIyZGEwIiwiN2MzYWU4NzctNTU0Yy01Mzg4LWJhNTgtMWUyMzA0MTAxZmIxIiwiN2M5MjM4NGQtYjFlNy01MmNlLWE4ZTMtODdkN2U3ZjlhYjVjIiwiN2U2ODVlNTItMmQyOS01NzcxLTllZTItMTU1M2NkYTJjNWNiIiwiN2U3NGYzYWYtZWM5MS01YmE2LTg1NjUtYzU5YmIzYjk4NDRmIiwiN2YwYTYzYjctYjNjOC01MzAwLWI3ZGItZGY3NzIzNmRjNTM4IiwiN2Y4ZGRlNzktYzgzNy01MTBhLTllMDctNDRkM2EyZjkzOTU4IiwiODFiODQ2NDUtNGY2YS01YTY5LWFjYTEtMWIxYWVlZTBmZGI5IiwiODQ4ZjQ3NTUtYzJjMy01ZDgzLWI2NjEtOWJhNGEyYzI1MWYzIiwiODUwOWZjZmMtZjM4Ni01ZTIzLTliOTAtNzZjNmZmNTc2ZjQ3IiwiODc3MGNhNmQtY2Y2OS01MDM5LWJhNjEtYWFiZmVhY2NjMmMxIiwiODg5ZjA2NTctNGY3NS01MjYyLTg2NWYtY2RiOGNmMDYyNWU3IiwiOGIwZjFjMGEtNDcxZS01NDUyLThkZjctODJjZTVlZjIzZDNmIiwiOGI3NDI5NWQtYmNiNy01MTFlLTgxMTQtOTIyOGM1M2RjMDZjIiwiOGZjYzg3ODYtOTA4ZS01MzNhLTljNGYtZGIwNTY1MjYwMDQxIiwiOTA3NmRlZDUtZTcxMC01YzUwLTg4ZTQtMmQ3MDRiMWNhMTA0IiwiOTEyNmE3MzAtNzg0Yi01YjIzLThjMmYtZTdlMjNiMTk5NWNhIiwiOTE1OWY2MGItYmNiOC01YTU0LWE2YTEtMzZmNjZhMDY2ZmZhIiwiOTM0OGE1ZWYtMTk2MS01MmQ4LTlhYTQtOWIxMTZlOTg4OWQxIiwiOTRkODRiNzAtNjlmMy01NGMwLTg5NzgtYWE4OGI3MDc2YmU1IiwiOTRmYmVkMmItMmI2NS01MThmLTk4NTAtMWY2YWM5NWI2OTQ5IiwiOTY5OWQ0MzktOTYxOS01ZWViLWEyMDUtN2UzY2VmZGVlY2U4IiwiOTg1Y2I3ODMtZWJmNy01NzVkLWI1MWEtNzExYjVhY2Y5MDJmIiwiOTkwOWIwNTEtYzcyMC01OTg3LTllOTYtMjlkODg5OTEzZDBlIiwiOTkzZGUxNGMtZTNmMS01ZWYwLTg2MmYtOTQxZDgzODdjNjZlIiwiOWE1MTM4MzEtN2ZmOC01MDVmLWI1ZjUtYjBmMTQ3MWZjNjJiIiwiOWI3OWU3MzctZTQ0My01YTEzLWEzMmEtZDI1MjkwYmMwZTI4IiwiOWMyMDZmOWYtYWEzYS01M2VlLTgyY2ItNGUxNzk4OGFmNjQ5IiwiOWY0N2RjMjUtMjQ2My01Mzc2LTk0OTUtZWIwYzc0MjVjNWVlIiwiYTFhNmExYTAtNWE2Ny01ODE2LTlkYjQtNWQ5NDk5ZWUyY2UyIiwiZTlhZmU5OWQtZWE5MC01NTEzLWJlOWUtNzM0ZmVhN2Y1MjkzIl0sIi9wZXJtaXNzaW9ucyI6WyI2MmU5ZTk5Yy0yZGUyLTE4ZjktYTc1NC0yN2VmMDAwMDAwMDBfQUNDT1VOVFMiLCJ0ZXN0QHRlc3QuY29tX0VNQUlMIiwid2hlbjpzZWxmOjYyZTllOTljLTJkZTItMThmOS1hNzU0LTI3ZWYwMDAwMDAwMDpncmFudDphY2NvdW50czphbGw6Iiwid2hlbjpzZWxmOjYyZTllOTljLTJkZTItMThmOS1hNzU0LTI3ZWYwMDAwMDAwMDpncmFudDpjbGllbnRzOmFsbDoiLCJ3aGVuOnNlbGY6NjJlOWU5OWMtMmRlMi0xOGY5LWE3NTQtMjdlZjAwMDAwMDAwOmdyYW50OmNvbXBhbmllczphbGw6Iiwid2hlbjpzZWxmOjYyZTllOTljLTJkZTItMThmOS1hNzU0LTI3ZWYwMDAwMDAwMDpncmFudDpmaWxlczphbGw6Iiwid2hlbjpzZWxmOjYyZTllOTljLTJkZTItMThmOS1hNzU0LTI3ZWYwMDAwMDAwMDpncmFudDppbnZvaWNlczphbGw6Iiwid2hlbjpzZWxmOjYyZTllOTljLTJkZTItMThmOS1hNzU0LTI3ZWYwMDAwMDAwMDpncmFudDpwYXltZW50YWRkcmVzc2VzOmFsbDoiLCJ3aGVuOnNlbGY6NjJlOWU5OWMtMmRlMi0xOGY5LWE3NTQtMjdlZjAwMDAwMDAwOmdyYW50OnBheW1lbnRtZXRob2RzOmFsbDoiLCJ3aGVuOnNlbGY6NjJlOWU5OWMtMmRlMi0xOGY5LWE3NTQtMjdlZjAwMDAwMDAwOmdyYW50OnBheW1lbnRzOmFsbDoiLCJ3aGVuOnNlbGY6NjJlOWU5OWMtMmRlMi0xOGY5LWE3NTQtMjdlZjAwMDAwMDAwOmdyYW50OnByb2plY3RzOmFsbDoiLCJ3aGVuOnNlbGY6NjJlOWU5OWMtMmRlMi0xOGY5LWE3NTQtMjdlZjAwMDAwMDAwOmdyYW50Om1lc3NhZ2VzOmFsbDoiLCJ3aGVuOnNlbGY6NjJlOWU5OWMtMmRlMi0xOGY5LWE3NTQtMjdlZjAwMDAwMDAwOmdyYW50OnRyYWNraW5nczphbGw6Iiwid2hlbjpzZWxmOjYyZTllOTljLTJkZTItMThmOS1hNzU0LTI3ZWYwMDAwMDAwMDpncmFudDp0aW1lY2xvY2tlbnRyaWVzOmFsbDoiXSwiaXNzIjoiaHR0cHM6Ly9kZXYtYXV0aC5pbmNsdWRlLmNvbS8iLCJzdWIiOiJhdXRoMHw2MmU5ZTk5YzJkZTIxOGY5YTc1NDI3ZWYiLCJhdWQiOlsiaHR0cHM6Ly9kZXYtcG9ydGFsLWFwaS5pbmNsdWRlLmNvbSIsImh0dHBzOi8vZGV2LXJ2amEtbjF3LnVzLmF1dGgwLmNvbS91c2VyaW5mbyJdLCJpYXQiOjE3NDgyMzg2NzksImV4cCI6MTc0ODI0NTg3OSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsImF6cCI6ImgyZDBvbmxmODVOaEwyb3JKRm5INjBtaFU0YUdibU5iIiwicGVybWlzc2lvbnMiOltdfQ.Afbb7TMrdR_P6qkrWr4opPe1icJt398Q3llhJseqKRTsULHmE8iGe8WKO7duhU1aof4ssU_MTWa5N08RJ84-7QtWm9M7UwNXDglYIgCAqsrMYHvta4FPr6kL0eUKr5ZMkRS8euO39EsDir9UWGK_ObHreILVtwuwVxQU8jpJBAAAerk1oudNheaxkQ6W88C3ESii2C2OA8E0p04yRCSIqzTOZT6D68duQs00sfcCTQYs6rdDeTNb3fH61tIIo8erBLjlKPbIM7N_-NyPOkmoQB-pbwWbtHkTwMTnQVj-2IjmQY9-c4dn8fr0bVWsgw7m5UBPmD7WgEhVln2xawccOQ";

        public SyncGOController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found.");
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("tenants")]
        public async Task<IActionResult> SyncTenants()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int currentPage = 1;
                int pageSize = 100; // Adjust page size as needed
                int totalPages = 1;
                int totalRecordsSynced = 0;

                do
                {
                    var url = $"{ApiBaseUrl}/admin/tenants?page[number]={currentPage}&page[size]={pageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, $"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<TenantApiResponse>(content);

                    if (apiResponse?.Data == null || !apiResponse.Data.Any())
                    {
                        break;
                    }

                    // Update total pages from first response
                    if (currentPage == 1)
                    {
                        totalPages = apiResponse.Meta.TotalPages;
                    }

                    using (IDbConnection db = new SqlConnection(_connectionString))
                    {
                        foreach (var tenant in apiResponse.Data)
                        {
                            var sql = @"
                                IF NOT EXISTS (SELECT 1 FROM [dbo].[Tenants] WHERE Id = @Id)
                                BEGIN
                                    INSERT INTO [dbo].[Tenants] (Id, name, owner_email, owner_name, created_at, enabled)
                                    VALUES (@Id, @Name, @OwnerEmail, @OwnerName, @CreatedAt, 1)
                                END";

                            await db.ExecuteAsync(sql, new
                            {
                                Id = Guid.Parse(tenant.Id),
                                Name = tenant.Attributes.Name,
                                OwnerEmail = tenant.Attributes.OwnerEmail,
                                OwnerName = tenant.Attributes.OwnerName,
                                CreatedAt = tenant.Attributes.CreatedAt
                            });
                        }
                    }

                    totalRecordsSynced += apiResponse.Data.Count;
                    currentPage++;

                } while (currentPage <= totalPages);

                return Ok(new
                {
                    Message = "Sync completed successfully",
                    TotalRecordsSynced = totalRecordsSynced
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("companies")]
        public async Task<IActionResult> SyncCompanies()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var url = $"{ApiBaseUrl}/admin/companies";
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"API request failed: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CompanyApiResponse>(content);

                if (apiResponse?.Data == null || !apiResponse.Data.Any())
                {
                    throw new Exception("No data found in the API response.");
                }

                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    foreach (var company in apiResponse.Data)
                    {
                        var sql = @"
                                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE id = @Id)
                                BEGIN
                                    INSERT INTO [dbo].[Companies] (id, tenant_id, name, enabled, is_deleted, created_at)
                                    VALUES (@Id, @TenantId, @Name, @Enabled, @IsDeleted, @CreatedAt)
                                END
                                ELSE
                                BEGIN
                                    UPDATE [dbo].[Companies]
                                    SET tenant_id = @TenantId,
                                        name = @Name,
                                        enabled = @Enabled,
                                        is_deleted = @IsDeleted,
                                        updated_at = GETDATE()
                                    WHERE id = @Id
                                END";

                        await db.ExecuteAsync(sql, new
                        {
                            Id = Guid.Parse(company.Id),
                            TenantId = Guid.Parse(company.Attributes.TenantsId),
                            Name = company.Attributes.Name,
                            Enabled = company.Attributes.Enabled,
                            IsDeleted = company.Attributes.IsDeleted,
                            CreatedAt = company.Attributes.CreatedAt
                        });
                    }
                }

                return Ok(new
                {
                    Message = "Companies sync completed successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("projects")]
        public async Task<IActionResult> SyncProjects([FromQuery] string tenants_id = null, [FromQuery] string companies_id = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int currentPage = 1;
                int pageSize = 100;
                int totalRecordsSynced = 0;

                do
                {
                    var url = $"{ApiBaseUrl}/includego/tenants/{tenants_id}/companies/{companies_id}/projects?page[number]={currentPage}&page[size]={pageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, $"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ProjectApiResponse>(content);

                    if (apiResponse?.Data == null || !apiResponse.Data.Any())
                    {
                        break;
                    }

                    using (IDbConnection db = new SqlConnection(_connectionString))
                    {
                        foreach (var project in apiResponse.Data)
                        {
                            var sql = @"
                                IF NOT EXISTS (SELECT 1 FROM [dbo].[Projects] WHERE Id = @Id)
                                BEGIN
                                    INSERT INTO [dbo].[Projects] (
                                        id, tenants_id, companies_id, external_project_id, external_division_id,
                                        project_name, locations_id, summary_description, detail_description,
                                        workflows_id, workflow_state, clients_id, project_manager_id,
                                        start_date_target, end_date_target, resource_projects_id, resource_tasks_id,
                                        travel_projects_id, travel_tasks_id, divisions_id, branch_id,
                                        default_tax_authority_id, clients_name, parent_projects_id, deleted
                                    )
                                    VALUES (
                                        @Id, @TenantsId, @CompaniesId, @ExternalProjectId, @ExternalDivisionId,
                                        @ProjectName, @LocationsId, @SummaryDescription, @DetailDescription,
                                        @WorkflowsId, @WorkflowState, @ClientsId, @ProjectManagerId,
                                        @StartDateTarget, @EndDateTarget, @ResourceProjectsId, @ResourceTasksId,
                                        @TravelProjectsId, @TravelTasksId, @DivisionsId, @BranchId,
                                        @DefaultTaxAuthorityId, @ClientsName, @parent_projects_id, @deleted
                                    )
                                END";

                            var locationsId = project.Relationships.Locations.Data.FirstOrDefault()?.Id;
                            var clientsId = project.Relationships.Clients.Data.FirstOrDefault()?.Id;
                            var workflowsId = project.Relationships.Workflows.Data.FirstOrDefault()?.Id;

                            await db.ExecuteAsync(sql, new
                            {
                                Id = Guid.Parse(project.Id),
                                TenantsId = Guid.Parse(tenants_id),
                                CompaniesId = Guid.Parse(companies_id),
                                ExternalProjectId = project.Attributes.ExternalProjectId,
                                ExternalDivisionId = project.Attributes.ExternalDivisionId,
                                ProjectName = project.Attributes.ProjectName,
                                LocationsId = locationsId != null ? Guid.Parse(locationsId) : (Guid?)null,
                                SummaryDescription = project.Attributes.SummaryDescription,
                                DetailDescription = project.Attributes.DetailDescription,
                                WorkflowsId = workflowsId != null ? Guid.Parse(workflowsId) : Guid.Empty,
                                WorkflowState = project.Attributes.WorkflowState,
                                ClientsId = clientsId != null ? Guid.Parse(clientsId) : Guid.Empty,
                                ProjectManagerId = project.Attributes.ProjectManagerId,
                                StartDateTarget = project.Attributes.StartDateTarget,
                                EndDateTarget = project.Attributes.EndDateTarget,
                                ResourceProjectsId = !string.IsNullOrEmpty(project.Attributes.ResourceProjectsId) ? Guid.Parse(project.Attributes.ResourceProjectsId) : (Guid?)null,
                                ResourceTasksId = !string.IsNullOrEmpty(project.Attributes.ResourceTasksId) ? Guid.Parse(project.Attributes.ResourceTasksId) : (Guid?)null,
                                TravelProjectsId = !string.IsNullOrEmpty(project.Attributes.TravelProjectsId) ? Guid.Parse(project.Attributes.TravelProjectsId) : (Guid?)null,
                                TravelTasksId = !string.IsNullOrEmpty(project.Attributes.TravelTasksId) ? Guid.Parse(project.Attributes.TravelTasksId) : (Guid?)null,
                                DivisionsId = project.Attributes.DivisionsId,
                                BranchId = project.Attributes.BranchId,
                                DefaultTaxAuthorityId = project.Attributes.DefaultTaxAuthorityId,
                                ClientsName = project.Attributes.ClientsName,
                                parent_projects_id = project.Attributes.parent_projects_id.HasValue ? (Guid?)project.Attributes.parent_projects_id.Value : null,
                                deleted = project.Attributes.deleted.HasValue ? (bool?)project.Attributes.deleted.Value : null
                            });
                        }
                    }

                    totalRecordsSynced += apiResponse.Data.Count;
                    currentPage++;

                } while (true);

                return Ok(new
                {
                    Message = "Projects sync completed successfully",
                    TotalRecordsSynced = totalRecordsSynced
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}