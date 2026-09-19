<script>
    import { fly } from "svelte/transition";
    let { text, role, loading } = $props();
</script>

<div class="message {role}" transition:fly={{ y: 12, duration: 250 }}>
    {#if loading}
        <div class="loading-dots">
            <span>•</span>
            <span>•</span>
            <span>•</span>
        </div>
    {:else}
        <p>{text}</p>
    {/if}
</div>

<style>
    .message {
        max-width: 70%;
        padding: 10px 16px;
        border-radius: 18px;
        line-height: 1.5;
    }

    .message p {
        margin: 0;
    }

    .message.assistant {
        align-self: flex-start;
        background: var(--code-bg, #f4f3ec);
        color: var(--text-h, #08060d);
    }

    .message.user {
        align-self: flex-end;
        background: var(--text-h, #edece7);
        color: var(--text-h, #08060d);
    }

    .loading-dots {
        display: inline-flex;
        align-items: center;
        gap: 3px;
    }

    .loading-dots span {
        display: inline-block;
        animation: pulse 1.2s infinite ease-in-out;
    }

    .loading-dots span:nth-child(1) {
        animation-delay: 0s;
    }

    .loading-dots span:nth-child(2) {
        animation-delay: 0.2s;
    }

    .loading-dots span:nth-child(3) {
        animation-delay: 0.4s;
    }

    @keyframes pulse {
        0%,
        60%,
        100% {
            opacity: 0.3;
            transform: translateY(0);
        }
        30% {
            opacity: 1;
            transform: translateY(-3px);
        }
    }
</style>
