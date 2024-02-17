using OicdDemo.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace OicdDemo.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FileController : ControllerBase
{
    private static List<FileDto> _files = new List<FileDto>()
    {
        new FileDto()
        {
            Id = 1,
            FriendlyName = "File 1",
            HashedName = Guid.NewGuid().ToString(),
        },
        new FileDto()
        {
            Id = 2,
            FriendlyName = "File 2",
            HashedName = Guid.NewGuid().ToString(),
        }
    };


    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create([FromBody] FileDto file, CancellationToken cancellationToken)
    {
        file.Id = !_files.Any() ? 1 : _files.Max(x => x.Id) + 1;
        _files.Add(file);
        return Ok(file);
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(_files);
    }

    [HttpPut]
    [Route("Update")]
    public async Task<IActionResult> Update([FromBody] FileDto file, CancellationToken cancellationToken)
    {
        var existingFile = _files.FirstOrDefault(f => f.Id == file.Id);
        if(existingFile != null)
        {
            existingFile.FriendlyName = file.FriendlyName;
            existingFile.HashedName = file.HashedName;
        }
        return Ok();
    }

    [HttpDelete]
    [Route("Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        _files = _files.Where(f => f.Id != id).ToList();
        return Ok();
    }

}
