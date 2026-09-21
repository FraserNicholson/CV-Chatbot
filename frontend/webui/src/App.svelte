<script lang="ts">
    import Message from "./lib/Message.svelte";
    import ChatInput from "./lib/ChatInput.svelte";

    type Message = {
        id: string;
        text: string;
        role: "user" | "assistant";
        loading?: boolean;
    };

    let messages = $state<Message[]>([]);
    let question = $state("");
    let isLoading = $state(false);

    async function ask() {
        const userText = question.trim();
        if (!userText) return;

        messages.push({
            id: crypto.randomUUID(),
            text: userText,
            role: "user",
        });
        question = "";
        isLoading = true;

        try {
            const res = await fetch(
                `${import.meta.env.VITE_API_URL}/cvchatbot/ask-question`,
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ query: userText }),
                },
            );

            if (!res.ok)
                throw new Error(
                    `Request failed: ${res.status} with message : ${res.body}`,
                );

            const data = await res.json();
            messages.push({
                id: crypto.randomUUID(),
                text: data.response,
                role: "assistant",
            });
        } catch (err) {
            messages.push({
                id: crypto.randomUUID(),
                text: "Something went wrong — please try again.",
                role: "assistant",
            });
        } finally {
            isLoading = false;
        }
    }
</script>

<div class="message-list">
    {#each messages as message (message.id)}
        <Message text={message.text} role={message.role} loading={false} />
    {/each}

    {#if isLoading}
        <Message text="" loading={true} role="assistant" />
    {/if}
</div>

<ChatInput bind:value={question} onSubmit={ask} />

<style>
    .message-list {
        display: flex;
        flex-direction: column;
        gap: 10px;
        width: min(820px, calc(100% - 48px));
        margin: 0 auto;
        padding: 24px 0 100px;
    }
</style>
