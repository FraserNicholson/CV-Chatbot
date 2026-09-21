<script lang="ts">
    let { value = $bindable(""), onSubmit } = $props();
    let textareaEl: HTMLTextAreaElement | undefined;

    function handleKeydown(e: KeyboardEvent) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            onSubmit();
        }
    }

    function autoGrow() {
        if (textareaEl == undefined) {
            return;
        }

        textareaEl.style.height = "auto";
        textareaEl.style.height = textareaEl.scrollHeight + "px";
    }

    $effect(() => {
        if (value === "" && textareaEl) {
            textareaEl.style.height = "auto";
        }
    });
</script>

<div class="input-bar">
    <textarea
        bind:this={textareaEl}
        bind:value
        onkeydown={handleKeydown}
        oninput={autoGrow}
        rows="1"
        placeholder="Ask about my experience..."
    ></textarea>
    <button onclick={onSubmit} disabled={!value.trim()} aria-label="Send">
        ↑
    </button>
</div>

<style>
    .input-bar {
        position: fixed;
        bottom: 24px;
        left: 50%;
        transform: translateX(-50%);
        width: min(820px, calc(100% - 48px));
        display: flex;
        align-items: center;
        gap: 8px;
        background: var(--bg, #fff);
        border: 1px solid var(--border, #e5e4e7);
        border-radius: 26px;
        padding: 10px 10px 10px 20px;
        box-shadow: var(--shadow);
    }

    textarea {
        flex: 1;
        border: none;
        outline: none;
        resize: none;
        background: transparent;
        font: inherit;
        max-height: 200px;
        overflow-y: auto;
        padding: 8px 0;
    }

    button {
        flex-shrink: 0;
        width: 36px;
        height: 36px;
        border-radius: 50%;
        border: none;
        background: var(--accent, #b8860b);
        color: #fff;
        font-size: 18px;
        line-height: 1;
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    button:disabled {
        background: var(--border, #e5e4e7);
        cursor: not-allowed;
    }
</style>
