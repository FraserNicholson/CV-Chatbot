<script>
    const submitForm = async (event) => {
        const formData = new FormData(event.target);

        // [Use formData]
        console.log([...formData]);

        const inputElement = document.getElementById("input");
        inputElement.value = "";
    };
</script>

<h1>Welcome to Fraser Nicholson's CV Chatbot</h1>
<form onsubmit={submitForm}>
    <textarea
        id="input"
        name="input"
        placeholder="Ask me a question about Fraser Nicholson"
    >
    </textarea>
    <input type="submit" value="Submit" />
</form>
