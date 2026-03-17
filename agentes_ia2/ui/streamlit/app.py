import sys
import os

sys.path.insert(0, os.path.join(os.path.dirname(__file__), "..", "src"))

import pandas as pd
import streamlit as st

from agente_sql.services import sql_service

# ── Configuración de página ──────────────────────────────────────────────────
st.set_page_config(
    page_title="Agente SQL",
    page_icon="🤖",
    layout="wide",
    initial_sidebar_state="expanded",
)

# ── Estilos CSS premium ──────────────────────────────────────────────────────
st.markdown("""
<style>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

html, body, [class*="css"] {
    font-family: 'Inter', sans-serif;
}

/* Header principal */
.main-header {
    background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f3460 100%);
    border-radius: 16px;
    padding: 2rem 2.5rem;
    margin-bottom: 2rem;
    border: 1px solid #334155;
    box-shadow: 0 4px 24px rgba(0,0,0,0.3);
}
.main-header h1 {
    color: #f8fafc;
    font-size: 2rem;
    font-weight: 700;
    margin: 0;
    letter-spacing: -0.5px;
}
.main-header p {
    color: #94a3b8;
    margin: 0.5rem 0 0;
    font-size: 1rem;
}

/* Input area */
.query-box textarea {
    background: #1e293b !important;
    border: 1px solid #334155 !important;
    border-radius: 12px !important;
    color: #f1f5f9 !important;
    font-size: 1rem !important;
}

/* SQL block */
.sql-block {
    background: #0f172a;
    border: 1px solid #1d4ed8;
    border-left: 4px solid #3b82f6;
    border-radius: 10px;
    padding: 1rem 1.25rem;
    font-family: 'Courier New', monospace;
    color: #93c5fd;
    font-size: 0.9rem;
    margin: 0.75rem 0;
    white-space: pre-wrap;
}

/* Stat badge */
.stat-badge {
    display: inline-block;
    background: #1d4ed8;
    color: #dbeafe;
    border-radius: 20px;
    padding: 0.2rem 0.75rem;
    font-size: 0.8rem;
    font-weight: 600;
    margin: 0.25rem 0.25rem 0.75rem 0;
}

/* History card */
.history-card {
    background: #1e293b;
    border: 1px solid #334155;
    border-radius: 10px;
    padding: 0.75rem 1rem;
    margin-bottom: 0.5rem;
    cursor: pointer;
    transition: border-color 0.2s;
}
.history-card:hover { border-color: #3b82f6; }
.history-q { color: #e2e8f0; font-size: 0.875rem; font-weight: 500; }
.history-meta { color: #64748b; font-size: 0.75rem; margin-top: 0.2rem; }

/* Sidebar */
section[data-testid="stSidebar"] {
    background: #0f172a;
    border-right: 1px solid #1e293b;
}

/* Botón principal */
.stButton > button {
    background: linear-gradient(135deg, #1d4ed8, #2563eb) !important;
    color: white !important;
    border: none !important;
    border-radius: 10px !important;
    padding: 0.6rem 2rem !important;
    font-weight: 600 !important;
    font-size: 1rem !important;
    transition: all 0.2s !important;
    box-shadow: 0 2px 12px rgba(37,99,235,0.4) !important;
}
.stButton > button:hover {
    transform: translateY(-1px) !important;
    box-shadow: 0 4px 20px rgba(37,99,235,0.55) !important;
}
</style>
""", unsafe_allow_html=True)


# ── Session state ────────────────────────────────────────────────────────────
if "historial" not in st.session_state:
    st.session_state.historial = []

if "schema_cache" not in st.session_state:
    try:
        st.session_state.schema_cache = sql_service.obtener_schema()
    except Exception as e:
        st.session_state.schema_cache = None
        st.session_state.schema_error = str(e)


# ── Sidebar ──────────────────────────────────────────────────────────────────
with st.sidebar:
    st.markdown("### 🤖 Agente SQL")
    st.markdown("---")

    schema_data = st.session_state.schema_cache
    if schema_data:
        st.success(f"✅ Conectado")
        st.metric("Tablas disponibles", schema_data.total_tablas)

        with st.expander("📋 Ver esquema de la BD", expanded=False):
            for tabla, columnas in schema_data.tablas.items():
                st.markdown(f"**`{tabla}`**")
                for col in columnas:
                    st.markdown(f"  - `{col}`")
    else:
        error_msg = getattr(st.session_state, "schema_error", "Error desconocido")
        st.error(f"❌ Sin conexión\n\n`{error_msg}`")

    st.markdown("---")
    st.markdown("##### 📜 Historial")
    if not st.session_state.historial:
        st.caption("Aún no hay consultas.")
    else:
        for i, item in enumerate(reversed(st.session_state.historial[-10:])):
            filas = item["filas"]
            st.markdown(
                f"""<div class="history-card">
                    <div class="history-q">💬 {item['pregunta'][:55]}{'…' if len(item['pregunta']) > 55 else ''}</div>
                    <div class="history-meta">{filas} fila{'s' if filas != 1 else ''} · #{len(st.session_state.historial) - i}</div>
                </div>""",
                unsafe_allow_html=True,
            )

        if st.button("🗑️ Limpiar historial"):
            st.session_state.historial = []
            st.rerun()


# ── Header ───────────────────────────────────────────────────────────────────
st.markdown("""
<div class="main-header">
    <h1>🤖 Agente SQL con IA</h1>
    <p>Consulta tu base de datos en lenguaje natural · SQL Server · GPT-4o</p>
</div>
""", unsafe_allow_html=True)


# ── Input ────────────────────────────────────────────────────────────────────
col_input, col_btn = st.columns([5, 1])

with col_input:
    pregunta = st.text_area(
        "Escribe tu pregunta:",
        placeholder="Ej: ¿Cuántos clientes están activos? ¿Cuáles son los 10 productos más vendidos?",
        height=100,
        label_visibility="collapsed",
    )

with col_btn:
    st.markdown("<br>", unsafe_allow_html=True)
    ejecutar = st.button("⚡ Consultar", use_container_width=True)


# ── Ejecución ────────────────────────────────────────────────────────────────
if ejecutar:
    if not pregunta or not pregunta.strip():
        st.warning("⚠️ Por favor, escribe una pregunta antes de consultar.")
    else:
        with st.spinner("🔄 Generando SQL y consultando la base de datos..."):
            try:
                resultado = sql_service.procesar_pregunta(pregunta.strip())

                # Guardar en historial
                st.session_state.historial.append({
                    "pregunta": resultado.pregunta,
                    "sql": resultado.sql,
                    "resultados": resultado.resultados,
                    "filas": resultado.filas,
                })

                # ── Resultado ─────────────────────────────────────────────
                st.markdown("#### ✅ Resultado")

                col_a, col_b = st.columns(2)
                col_a.markdown(f'<span class="stat-badge">🗂️ {resultado.filas} filas</span>', unsafe_allow_html=True)

                with st.expander("🔍 Ver SQL generado", expanded=True):
                    st.markdown(
                        f'<div class="sql-block">{resultado.sql}</div>',
                        unsafe_allow_html=True,
                    )

                if resultado.resultados:
                    df = pd.DataFrame(resultado.resultados)
                    st.dataframe(
                        df,
                        use_container_width=True,
                        hide_index=True,
                    )

                    # Descarga CSV
                    csv = df.to_csv(index=False).encode("utf-8")
                    st.download_button(
                        label="⬇️ Descargar CSV",
                        data=csv,
                        file_name="resultado.csv",
                        mime="text/csv",
                    )
                else:
                    st.info("La consulta se ejecutó pero no devolvió resultados.")

            except ValueError as e:
                st.error(f"❌ **Consulta rechazada:** {e}")
            except RuntimeError as e:
                st.error(f"🔥 **Error del servidor:** {e}")
            except Exception as e:
                st.error(f"⚠️ **Error inesperado:** {e}")


# ── Placeholder inicial ───────────────────────────────────────────────────────
elif not st.session_state.historial:
    st.markdown("""
    <div style="text-align:center; padding: 3rem 1rem; color: #475569;">
        <div style="font-size: 3rem; margin-bottom: 1rem;">💬</div>
        <h3 style="color:#64748b; font-weight:500;">¿Qué quieres consultar?</h3>
        <p style="max-width: 420px; margin: 0 auto; font-size: 0.95rem;">
            Escribe tu pregunta arriba en español y el agente generará y ejecutará el SQL automáticamente.
        </p>
    </div>
    """, unsafe_allow_html=True)
