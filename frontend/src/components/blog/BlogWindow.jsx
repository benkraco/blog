function BlogWindow({ children }) {
  return (
    <section className="blogWindow">
      <div className="windowHeader">
        <div className="windowControls">
          <img src="" alt="Icon -" />
          <img src="" alt="Icon □" />
          <img src="" alt="Icon X" />
        </div>
      </div>

      <div className="blogWindowContent">{children}</div>
    </section>
  );
}

export default BlogWindow;
