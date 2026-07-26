import BlogWindow from "../components/blog/BlogWindow";
import PostList from "../components/blog/PostList";
import PageTitle from "../components/ui/PageTitle";

function Home() {
  return (
    <>
      <PageTitle title="Home" />
      <BlogWindow title="Home.exe">
        <PostList />
      </BlogWindow>
    </>
  );
}

export default Home;
