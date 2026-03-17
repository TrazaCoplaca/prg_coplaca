/**
 * Agente SQL – Web UI
 * Conecta con la API FastAPI en localhost:8000
 */

const API_BASE = 'http://localhost:8000';
const HISTORY_KEY = 'agente_sql_history';
const MAX_HISTORY = 20;

/* ── DOM refs ─────────────────────────────────────────────────────────── */
const form = document.getElementById('queryForm');
const questionInput = document.getElementById('questionInput');
const submitBtn = document.getElementById('submitBtn');
const btnText = submitBtn.querySelector('.btn-text');
const btnIcon = submitBtn.querySelector('.btn-icon');
const btnSpinner = submitBtn.querySelector('.btn-spinner');
const charCount = document.getElementById('charCount');
const statusBadge = document.getElementById('statusBadge');
const statusLabel = statusBadge.querySelector('.status-label');

const resultsSection = document.getElementById('resultsSection');
const responseCard = document.getElementById('responseCard');
const responseMessage = document.getElementById('responseMessage');
const responseMeta = document.getElementById('responseMeta');
const sqlCode = document.getElementById('sqlCode');
const copyBtn = document.getElementById('copyBtn');

const tableCard = document.getElementById('tableCard');
const tableHead = document.getElementById('tableHead');
const tableBody = document.getElementById('tableBody');
const tableRowsLabel = document.getElementById('tableRows');
const exportToggleBtn = document.getElementById('exportToggleBtn');
const exportDropdown = document.getElementById('exportDropdown');
const exportCsvBtn = document.getElementById('exportCsvBtn');
const exportExcelBtn = document.getElementById('exportExcelBtn');
const exportPdfBtn = document.getElementById('exportPdfBtn');

const errorBanner = document.getElementById('errorBanner');
const errorMessage = document.getElementById('errorMessage');
const errorClose = document.getElementById('errorClose');

const historySection = document.getElementById('historySection');
const historyList = document.getElementById('historyList');
const clearHistoryBtn = document.getElementById('clearHistoryBtn');

// Quick queries
const quickChips = document.querySelectorAll('.quick-chip');
const formCaja = document.getElementById('formCaja');
const formAlbaran = document.getElementById('formAlbaran');
const formSocio = document.getElementById('formSocio');
const inputCaja = document.getElementById('inputCaja');
const inputAlbaran = document.getElementById('inputAlbaran');
const inputSocio = document.getElementById('inputSocio');
const quickFormBtns = document.querySelectorAll('.quick-form-btn');

/* ── State ────────────────────────────────────────────────────────────── */
let currentResults = [];
let currentSQL = '';
let activeQuickForm = null;

/* ── Textarea auto-resize ─────────────────────────────────────────────── */
questionInput.addEventListener('input', () => {
  questionInput.style.height = 'auto';
  questionInput.style.height = questionInput.scrollHeight + 'px';
  charCount.textContent = questionInput.value.length;
});

/* ── Ctrl+Enter shortcut ──────────────────────────────────────────────── */
questionInput.addEventListener('keydown', (e) => {
  if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) {
    e.preventDefault();
    form.requestSubmit();
  }
});

/* ── Quick queries chips (NUEVO) ──────────────────────────────────────── */
quickChips.forEach(chip => {
  chip.addEventListener('click', () => {
    const queryType = chip.dataset.query;
    toggleQuickForm(queryType, chip);
  });
});

function toggleQuickForm(type, chipElement) {
  // Si ya está abierto el mismo, cerrarlo
  if (activeQuickForm === type) {
    closeAllQuickForms();
    return;
  }

  // Cerrar todos
  closeAllQuickForms();

  // Abrir el correspondiente
  activeQuickForm = type;
  chipElement.classList.add('active');

  switch (type) {
    case 'caja':
      formCaja.hidden = false;
      inputCaja.focus();
      break;
    case 'albaran':
      formAlbaran.hidden = false;
      inputAlbaran.focus();
      break;
    case 'socio':
      formSocio.hidden = false;
      inputSocio.focus();
      break;
  }
}

function closeAllQuickForms() {
  activeQuickForm = null;
  quickChips.forEach(chip => chip.classList.remove('active'));
  formCaja.hidden = true;
  formAlbaran.hidden = true;
  formSocio.hidden = true;
}

/* ── Quick form buttons ─────────────────────────────────────────────── */
quickFormBtns.forEach(btn => {
  btn.addEventListener('click', () => {
    const action = btn.dataset.action;
    handleQuickQuery(action);
  });
});

inputCaja.addEventListener('keydown', (e) => {
  if (e.key === 'Enter') {
    e.preventDefault();
    handleQuickQuery('buscar-caja');
  }
});

inputAlbaran.addEventListener('keydown', (e) => {
  if (e.key === 'Enter') {
    e.preventDefault();
    handleQuickQuery('buscar-albaran');
  }
});

inputSocio.addEventListener('keydown', (e) => {
  if (e.key === 'Enter') {
    e.preventDefault();
    handleQuickQuery('buscar-socio');
  }
});

async function executeQuickQuery(type, value) {
  const response = await fetch(`${API_BASE}/quick-query`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      type,
      value: Number(value)
    })
  });

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.detail || data.detalle || data.mensaje || 'Error en consulta rápida');
  }

  return data;
}

async function handleQuickQuery(action) {
  let type = '';
  let value = '';

  try {
    switch (action) {
      case 'buscar-caja':
        value = inputCaja.value.trim();
        if (!value) {
          showError('Por favor, introduce un código de caja.');
          inputCaja.focus();
          return;
        }
        type = 'buscar_caja';
        inputCaja.value = '';
        break;

      case 'buscar-albaran':
        value = inputAlbaran.value.trim();
        if (!value) {
          showError('Por favor, introduce un número de albarán.');
          inputAlbaran.focus();
          return;
        }
        type = 'cajas_de_albaran';
        inputAlbaran.value = '';
        break;

      case 'buscar-socio':
        value = inputSocio.value.trim();
        if (!value) {
          showError('Por favor, introduce un código de socio.');
          inputSocio.focus();
          return;
        }
        type = 'cajas_de_socio';
        inputSocio.value = '';
        break;

      default:
        showError('Acción rápida no soportada.');
        return;
    }

    closeAllQuickForms();
    setLoading(true);
    hideError();
    hideResults();

    const data = await executeQuickQuery(type, value);

    renderResults(data);

  } catch (error) {
    showError(error.message || 'Error ejecutando consulta rápida');
  } finally {
    setLoading(false);
  }
}

/* ── Close error ──────────────────────────────────────────────────────── */
errorClose.addEventListener('click', () => hideError());

/* ── Health check ─────────────────────────────────────────────────────── */
async function checkHealth() {
  try {
    const res = await fetch(`${API_BASE}/health`, { signal: AbortSignal.timeout(4000) });
    if (res.ok) {
      setStatus('online', 'API conectada');
    } else {
      setStatus('offline', 'API no disponible');
    }
  } catch {
    setStatus('offline', 'Sin conexión');
  }
}

function setStatus(state, label) {
  statusBadge.className = `status-badge ${state}`;
  statusLabel.textContent = label;
}

checkHealth();
// Re-check every 30 s
setInterval(checkHealth, 30_000);

/* ── Form submit ──────────────────────────────────────────────────────── */
form.addEventListener('submit', async (e) => {
  e.preventDefault();
  const pregunta = questionInput.value.trim();
  if (!pregunta) return;

  executeQuery(pregunta);
});

/* ── Execute query (refactorizado para reutilizar) ────────────────────── */
async function executeQuery(pregunta) {
  // Actualizar el textarea con la pregunta (si viene de quick query)
  questionInput.value = pregunta;
  questionInput.dispatchEvent(new Event('input'));

  setLoading(true);
  hideError();
  hideResults();

  try {
    const response = await fetch(`${API_BASE}/query`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ pregunta }),
      signal: AbortSignal.timeout(60_000),
    });

    if (!response.ok) {
      const err = await response.json().catch(() => ({}));
      showError(err.detalle || `Error ${response.status}: ${response.statusText}`);
      return;
    }

    const data = await response.json();
    renderResults(data);
    addToHistory(pregunta);

  } catch (err) {
    if (err.name === 'TimeoutError') {
      showError('La consulta tardó demasiado. Intenta de nuevo con una pregunta más específica.');
    } else if (err.name === 'TypeError') {
      showError('No se pudo conectar con la API. Asegúrate de que el servidor está corriendo en localhost:8000.');
      setStatus('offline', 'Sin conexión');
    } else {
      showError(err.message || 'Error inesperado.');
    }
  } finally {
    setLoading(false);
  }
}

/* ── Render results ───────────────────────────────────────────────────── */
function renderResults(data) {
  // Response message
  responseMessage.textContent = data.mensaje || '—';
  responseMeta.textContent = `${data.filas} fila${data.filas !== 1 ? 's' : ''}`;

  // SQL
  currentSQL = data.sql || '';
  sqlCode.textContent = formatSQL(currentSQL);

  // Table
  currentResults = data.resultados || [];
  if (currentResults.length > 0) {
    renderTable(currentResults);
    tableCard.hidden = false;
  } else {
    tableCard.hidden = true;
  }

  resultsSection.hidden = false;
  resultsSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

function formatSQL(sql) {
  // Basic keyword uppercasing for readability
  const keywords = ['SELECT', 'FROM', 'WHERE', 'JOIN', 'LEFT JOIN', 'RIGHT JOIN', 'INNER JOIN',
    'GROUP BY', 'ORDER BY', 'HAVING', 'LIMIT', 'INSERT', 'UPDATE', 'DELETE', 'ON', 'AND', 'OR',
    'AS', 'DISTINCT', 'COUNT', 'SUM', 'AVG', 'MAX', 'MIN', 'TOP', 'WITH', 'UNION', 'EXCEPT', 'INTERSECT'];
  let formatted = sql;
  keywords.forEach(kw => {
    const re = new RegExp(`\\b${kw}\\b`, 'gi');
    formatted = formatted.replace(re, kw);
  });
  return formatted;
}

function renderTable(rows) {
  const cols = Object.keys(rows[0]);

  // Head
  tableHead.innerHTML = '<tr>' +
    cols.map(c => `<th>${escHtml(c)}</th>`).join('') +
    '</tr>';

  // Body
  tableBody.innerHTML = rows.map(row =>
    '<tr>' +
    cols.map(c => `<td title="${escHtml(String(row[c] ?? ''))}">${escHtml(formatCell(row[c]))}</td>`).join('') +
    '</tr>'
  ).join('');

  tableRowsLabel.textContent = `${rows.length} fila${rows.length !== 1 ? 's' : ''}`;
}

function formatCell(val) {
  if (val === null || val === undefined) return '—';
  return String(val);
}

function escHtml(str) {
  return str
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

/* ── Copy SQL ─────────────────────────────────────────────────────────── */
copyBtn.addEventListener('click', async () => {
  if (!currentSQL) return;
  try {
    await navigator.clipboard.writeText(currentSQL);
    copyBtn.classList.add('copied');
    copyBtn.querySelector('span').textContent = '¡Copiado!';
    setTimeout(() => {
      copyBtn.classList.remove('copied');
      copyBtn.querySelector('span').textContent = 'Copiar';
    }, 2000);
  } catch {
    // Fallback
    const ta = document.createElement('textarea');
    ta.value = currentSQL;
    document.body.appendChild(ta);
    ta.select();
    document.execCommand('copy');
    ta.remove();
  }
});

/* ── Export Menu Toggle ────────────────────────────────────────────────── */
exportToggleBtn.addEventListener('click', (e) => {
  e.stopPropagation();
  exportDropdown.hidden = !exportDropdown.hidden;
});

// Cerrar dropdown al hacer click fuera
document.addEventListener('click', (e) => {
  if (!exportToggleBtn.contains(e.target) && !exportDropdown.contains(e.target)) {
    exportDropdown.hidden = true;
  }
});

/* ── Export CSV ───────────────────────────────────────────────────────── */
exportCsvBtn.addEventListener('click', () => {
  if (!currentResults.length) return;

  exportDropdown.hidden = true;

  const cols = Object.keys(currentResults[0]);
  const csv = [
    cols.join(','),
    ...currentResults.map(row =>
      cols.map(c => {
        const v = String(row[c] ?? '');
        return v.includes(',') || v.includes('"') || v.includes('\n')
          ? `"${v.replace(/"/g, '""')}"` : v;
      }).join(',')
    )
  ].join('\r\n');

  const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' });
  const a = document.createElement('a');
  a.href = URL.createObjectURL(blob);
  a.download = `consulta_${new Date().toISOString().slice(0, 19).replace(/:/g, '-')}.csv`;
  a.click();
  URL.revokeObjectURL(a.href);
});

/* ── Export Excel ─────────────────────────────────────────────────────── */
exportExcelBtn.addEventListener('click', async () => {
  if (!currentResults.length) {
    showError('No hay datos para exportar');
    return;
  }

  exportDropdown.hidden = true;

  try {
    const response = await fetch(`${API_BASE}/export/excel`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        data: currentResults,
        filename: `consulta_${new Date().toISOString().slice(0, 10)}`
      })
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({}));
      throw new Error(error.detail || error.detalle || 'Error al exportar a Excel');
    }

    // Descargar el archivo
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `consulta_${new Date().toISOString().slice(0, 19).replace(/:/g, '-')}.xlsx`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);

  } catch (error) {
    showError(error.message || 'Error al exportar a Excel');
  }
});

/* ── Export PDF ───────────────────────────────────────────────────────── */
exportPdfBtn.addEventListener('click', async () => {
  if (!currentResults.length) {
    showError('No hay datos para exportar');
    return;
  }

  exportDropdown.hidden = true;

  try {
    const response = await fetch(`${API_BASE}/export/pdf`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        data: currentResults,
        title: questionInput.value.trim() || 'Consulta SQL',
        filename: `consulta_${new Date().toISOString().slice(0, 10)}`
      })
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({}));
      throw new Error(error.detail || error.detalle || 'Error al exportar a PDF');
    }

    // Descargar el archivo
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `consulta_${new Date().toISOString().slice(0, 19).replace(/:/g, '-')}.pdf`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);

  } catch (error) {
    showError(error.message || 'Error al exportar a PDF');
  }
});

/* ── History ──────────────────────────────────────────────────────────── */
function getHistory() {
  try { return JSON.parse(localStorage.getItem(HISTORY_KEY)) || []; }
  catch { return []; }
}

function saveHistory(hist) {
  localStorage.setItem(HISTORY_KEY, JSON.stringify(hist));
}

function addToHistory(pregunta) {
  let hist = getHistory();
  // Remove duplicate if exists
  hist = hist.filter(h => h.q !== pregunta);
  hist.unshift({ q: pregunta, t: Date.now() });
  if (hist.length > MAX_HISTORY) hist = hist.slice(0, MAX_HISTORY);
  saveHistory(hist);
  renderHistory();
}

function renderHistory() {
  const hist = getHistory();
  if (!hist.length) {
    historySection.hidden = true;
    return;
  }
  historySection.hidden = false;
  historyList.innerHTML = hist.map((item, i) =>
    `<li class="history-item" data-idx="${i}" role="button" tabindex="0">
      <span class="history-item-dot"></span>
      <span class="history-item-q">${escHtml(item.q)}</span>
      <span class="history-item-time">${relTime(item.t)}</span>
    </li>`
  ).join('');

  historyList.querySelectorAll('.history-item').forEach(el => {
    el.addEventListener('click', () => {
      const idx = parseInt(el.dataset.idx, 10);
      const hist = getHistory();
      questionInput.value = hist[idx].q;
      questionInput.dispatchEvent(new Event('input'));
      questionInput.focus();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
    el.addEventListener('keydown', e => { if (e.key === 'Enter') el.click(); });
  });
}

clearHistoryBtn.addEventListener('click', () => {
  localStorage.removeItem(HISTORY_KEY);
  renderHistory();
});

function relTime(ts) {
  const diff = Date.now() - ts;
  const m = Math.floor(diff / 60_000);
  if (m < 1) return 'ahora';
  if (m < 60) return `hace ${m} min`;
  const h = Math.floor(m / 60);
  if (h < 24) return `hace ${h} h`;
  return `hace ${Math.floor(h / 24)} d`;
}

// Render history on load
renderHistory();

/* ── UI helpers ───────────────────────────────────────────────────────── */
function setLoading(loading) {
  submitBtn.disabled = loading;
  if (loading) {
    btnText && (btnText.hidden = true);
    btnIcon.hidden = true;
    btnSpinner.hidden = false;
  } else {
    btnText && (btnText.hidden = false);
    btnIcon.hidden = false;
    btnSpinner.hidden = true;
  }
}

function hideResults() {
  resultsSection.hidden = true;
  tableCard.hidden = true;

  // Limpiar datos anteriores para evitar que se queden en pantalla si falla o si CSS sobreescribe el hidden
  responseMessage.textContent = '—';
  responseMeta.textContent = '';
  sqlCode.textContent = '';
  currentSQL = '';
  currentResults = [];
  tableHead.innerHTML = '';
  tableBody.innerHTML = '';
  tableRowsLabel.textContent = '';
}

function showError(msg) {
  errorMessage.textContent = msg;
  errorBanner.hidden = false;
  errorBanner.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

function hideError() {
  errorBanner.hidden = true;
}