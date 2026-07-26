import Equis from '../../assets/Icons/X.png'
import Cuadrado from '../../assets/Icons/Cuadrado.png'
import Menos from '../../assets/Icons/-.png'

function BlogWindow({ children, title }) {
  return (
    <section className="blogWindow">
      <div className="windowHeader">
        <h3>{title}</h3>
        <div className="windowControls">
          <img src={Menos} alt="Icon -" />
          <img src={Cuadrado} alt="Icon □" />
          <img src={Equis} alt="Icon X" />
        </div>
      </div>

      <div className="blogWindowContent">{children}</div>
    </section>
  );
}

export default BlogWindow;
