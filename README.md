# CV Chatbot

A CV Chatbot to answer questions on Fraser Nicholson's CV.

## Updating CV information

Details on updating CV information can be found in the [Chunk Embeddings Console App README](./backend/CVChatbotApi/ChunkEmbeddingsConsoleApp/README.md)

## Infrastructure

Terraform state is stored in a manually provisioned storage container, and container images are pushed to a public docker registry, with all other infrastructure provisioned and managed using terraform. See the `./infra` folder for more detail.

### Updating infrastructure

For any infrastructure changes, due to their being no CI checks on them, please run `terraform plan` BEFORE merging changes.

Once the changes are in main, then run `terraform apply`

## Manually building docker image

To manually push a docker image, run the following commands whilst in the `./backend/CVChatbotApi` folder:

```sh
docker build -f CVChatbotApi/Dockerfile -t frasernicholson/cv-chatbot-api:latest .

docker push frasernicholson/cv-chatbot-api:latest
```

To build a local image, and test it, add an env file with an `Gemini__ApiKey` entry, and run the following commands (in the same directory as above):

```sh
docker build -f CVChatbotApi/Dockerfile -t cv-chatbot-api:local .

docker run -p 8080:8080 --env-file .env cv-chatbot-api:local
```
