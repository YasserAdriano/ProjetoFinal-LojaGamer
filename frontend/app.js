document.addEventListener('DOMContentLoaded', () => {

    const apiUrl = 'http://localhost:5176'; 
    
    const loginContainer = document.getElementById('login-container');
    const dashboardContainer = document.getElementById('dashboard-container');
    const loginForm = document.getElementById('login-form');
    const loginEmail = document.getElementById('login-email');
    const loginSenha = document.getElementById('login-senha');
    const loginError = document.getElementById('login-error');
    
    const produtoForm = document.getElementById('produto-form');
    const produtoId = document.getElementById('produto-id');
    const produtoNome = document.getElementById('produto-nome');
    const produtoDescricao = document.getElementById('produto-descricao');
    const produtoPreco = document.getElementById('produto-preco');
    const produtoEstoque = document.getElementById('produto-estoque');
    const produtoError = document.getElementById('produto-error');
    
    const produtosTbody = document.getElementById('produtos-tbody');
    const logoutButton = document.getElementById('logout-button');

    let authToken = localStorage.getItem('token');

    async function fetchApi(endpoint, options = {}) {
        const token = localStorage.getItem('token');
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        options.headers = headers;
        
        try {
            const response = await fetch(apiUrl + endpoint, options);
            
            if (response.status === 204) {
                return true; 
            }
            
            const data = await response.json();

            if (!response.ok) {
                let errorMessage = `Erro: ${response.status}`;
                if (data && data.message) {
                    errorMessage = data.message;
                } else if (data && data.title) {
                    errorMessage = data.title;
                } else if (typeof data === 'string') {
                    errorMessage = data;
                }
                throw new Error(errorMessage);
            }
            
            return data;
        } catch (error) {
            console.error('Erro na API:', error);
            throw error;
        }
    }

    async function handleLogin(e) {
        e.preventDefault();
        loginError.textContent = '';
        
        const loginData = {
            email: loginEmail.value,
            senha: loginSenha.value
        };

        try {
            const data = await fetchApi('/api/auth/login', {
                method: 'POST',
                body: JSON.stringify(loginData)
            });

            if (data.token) {
                localStorage.setItem('token', data.token);
                authToken = data.token;
                showDashboard();
            }
        } catch (error) {
            loginError.textContent = error.message || 'Falha no login.';
        }
    }

    function handleLogout() {
        localStorage.removeItem('token');
        authToken = null;
        loginEmail.value = '';
        loginSenha.value = '';
        showLogin();
    }

    async function fetchProdutos() {
        produtosTbody.innerHTML = '';
        
        try {
            const produtos = await fetchApi('/api/produtos');
            produtos.forEach(produto => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${produto.id}</td>
                    <td>${produto.nome}</td>
                    <td>R$ ${produto.preco.toFixed(2)}</td>
                    <td>${produto.estoque}</td>
                    <td>
                        <button class="btn-edit" data-id="${produto.id}">Editar</button>
                        <button class="btn-delete" data-id="${produto.id}">Excluir</button>
                    </td>
                `;
                produtosTbody.appendChild(tr);
            });
        } catch (error) {
            alert('Falha ao carregar produtos.');
        }
    }

    async function handleProdutoSubmit(e) {
        e.preventDefault();
        produtoError.textContent = '';
        
        const produtoData = {
            nome: produtoNome.value,
            descricao: produtoDescricao.value,
            preco: parseFloat(produtoPreco.value),
            estoque: parseInt(produtoEstoque.value)
        };

        const id = produtoId.value;
        const method = id ? 'PUT' : 'POST';
        const endpoint = id ? `/api/produtos/${id}` : '/api/produtos';

        try {
            await fetchApi(endpoint, {
                method: method,
                body: JSON.stringify(produtoData)
            });
            
            produtoForm.reset();
            produtoId.value = '';
            fetchProdutos();
            
        } catch (error) {
            if (error.message.includes('403')) {
                produtoError.textContent = 'Acesso negado. Apenas Administradores podem fazer isso.';
            } else {
                produtoError.textContent = error.message || 'Falha ao salvar produto.';
            }
        }
    }

    function handleEditClick(e) {
        const id = e.target.dataset.id;
        const row = e.target.closest('tr');
        const cells = row.children;
        
        produtoId.value = id;
        produtoNome.value = cells[1].textContent;
        produtoPreco.value = parseFloat(cells[2].textContent.replace('R$ ', ''));
        produtoEstoque.value = parseInt(cells[3].textContent);
        
        const descricao = produtos.find(p => p.id == id)?.descricao || '';
        produtoDescricao.value = descricao;

        window.scrollTo(0, 0);
    }
    
    let produtos = []; 
    async function fetchProdutos() {
        produtosTbody.innerHTML = '';
        try {
            produtos = await fetchApi('/api/produtos'); 
            produtos.forEach(produto => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${produto.id}</td>
                    <td>${produto.nome}</td>
                    <td>R$ ${produto.preco.toFixed(2)}</td>
                    <td>${produto.estoque}</td>
                    <td>
                        <button class="btn-edit" data-id="${produto.id}">Editar</button>
                        <button class="btn-delete" data-id="${produto.id}">Excluir</button>
                    </td>
                `;
                produtosTbody.appendChild(tr);
            });
        } catch (error) {
            alert('Falha ao carregar produtos.');
        }
    }

    async function handleDeleteClick(e) {
        const id = e.target.dataset.id;
        if (!confirm(`Tem certeza que deseja excluir o produto ID ${id}?`)) {
            return;
        }

        try {
            await fetchApi(`/api/produtos/${id}`, {
                method: 'DELETE'
            });
            fetchProdutos();
        } catch (error) {
            if (error.message.includes('403')) {
                alert('Acesso negado. Apenas Administradores podem excluir.');
            } else {
                alert(error.message || 'Falha ao excluir produto.');
            }
        }
    }

    function showDashboard() {
        loginContainer.style.display = 'none';
        dashboardContainer.style.display = 'block';
        fetchProdutos();
    }

    function showLogin() {
        loginContainer.style.display = 'block';
        dashboardContainer.style.display = 'none';
    }

    loginForm.addEventListener('submit', handleLogin);
    logoutButton.addEventListener('click', handleLogout);
    produtoForm.addEventListener('submit', handleProdutoSubmit);
    
    produtosTbody.addEventListener('click', (e) => {
        if (e.target.classList.contains('btn-edit')) {
            handleEditClick(e);
        } else if (e.target.classList.contains('btn-delete')) {
            handleDeleteClick(e);
        }
    });

    if (authToken) {
        showDashboard();
    } else {
        showLogin();
    }
});