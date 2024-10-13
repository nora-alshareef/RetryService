using DelegateRetry;
using DelegateRetry.Services;
using Microsoft.AspNetCore.Mvc;

namespace TestDelegateRetry.testRetry
{
    [ApiController]
    [Route("[controller]")]
    public class OperationsController : ControllerBase   // Ensure you have the correct constructor syntax
    {
        private readonly IDelegateService _delegateService;

        public OperationsController(IDelegateService delegateService)
        {
            _delegateService = delegateService;
        }

        [HttpGet("test-func1")]
        public async Task<IActionResult> TestFunc1()
        {
            var rec1 = new Record1(1, new Record2('C'));
            var rec2 = new Record1(3, new Record2('D'));
            var result = await _delegateService.DelegateRetry(Someclass.Func1, rec1, rec2);
            return Ok($"Func1 is delegated: {result}");
        }

        [HttpGet("test-func2")]
        public async Task<IActionResult> TestFunc2()
        {
            await _delegateService.DelegateRetry(Someclass.Func2);
            return Ok("Func2 is delegated: executed without parameters");
        }

        [HttpGet("test-func3")]
        public async Task<IActionResult> TestFunc3(int a, int b)
        {
            var result = await _delegateService.DelegateRetry(Someclass.Func3, a, b);
            return Ok($"Func3 is delegated: result = {result}");
        }

        [HttpGet("test-func4")]
        public async Task<IActionResult> TestFunc4()
        {
            var records = new List<Record1>
            {
                new Record1(1, new Record2('A')),
                new Record1(2, new Record2('B'))
            };

            var result = await _delegateService.DelegateRetry(Someclass.Func4, records);
            return Ok($"Func4 is delegated: processed records = {string.Join(", ", result)}");
        }
    }
}