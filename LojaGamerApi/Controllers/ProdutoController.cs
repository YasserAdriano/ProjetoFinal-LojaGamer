using LojaGamerApi.Data;
using LojaGamerApi.Dtos;
using LojaGamerApi.Entities;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaGamerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class ProdutosController : ControllerBase
    {
        private readonly LojaGamerContext _context;

        public ProdutosController(LojaGamerContext context)
        {
            _context = context;
        }

        // --- 1. READ ---
        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> GetProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();
            return Ok(produtos);
        }

        // --- 2. READ (POR ID) ---
        [HttpGet("{id}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetProdutoPorId(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }
            return Ok(produto);
        }

        // --- 3. CREATE ---
        [HttpPost]
        [Authorize(Roles = "Administrador")] 
        public async Task<IActionResult> CreateProduto(ProdutoCreateDto produtoDto)
        {
            var produto = new Produto
            {
                Nome = produtoDto.Nome,
                Descricao = produtoDto.Descricao,
                Preco = produtoDto.Preco,
                Estoque = produtoDto.Estoque
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdutoPorId), new { id = produto.Id }, produto);
        }

        // --- 4. UPDATE ---
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")] 
        public async Task<IActionResult> UpdateProduto(int id, ProdutoUpdateDto produtoDto)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            if (produtoDto.Nome != null) produto.Nome = produtoDto.Nome;
            if (produtoDto.Descricao != null) produto.Descricao = produtoDto.Descricao;
            if (produtoDto.Preco.HasValue) produto.Preco = produtoDto.Preco.Value;
            if (produtoDto.Estoque.HasValue) produto.Estoque = produtoDto.Estoque.Value;

            await _context.SaveChangesAsync();
            return Ok(produto); 
        }

        // --- 5. DELETE ---
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] 
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}